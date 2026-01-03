import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { PatientService } from '../../services/patient.service';
import { CaregiverService } from '../../services/caregiver.service';
import { OfficeService } from '../../services/office.service';
import { AuthService } from '../../services/auth.service';
import { Patient, Caregiver, CreatePatient } from '../../models/patient.model';

@Component({
  selector: 'app-patient',
  templateUrl: './patient.component.html',
  styleUrls: ['./patient.component.css']
})
export class PatientComponent implements OnInit {
  patients: Patient[] = [];
  caregivers: Caregiver[] = [];
  offices: any[] = [];
  patientForm: FormGroup;
  editMode = false;
  editingPatientId: number | null = null;
  showForm = false;
  totalCount = 0;
  page = 1;
  pageSize = 10;
  loading = false;
  error = '';
  successMessage = '';
  sortBy = 'firstName';
  sortDescending = false;

  constructor(
    private patientService: PatientService,
    private caregiverService: CaregiverService,
    private officeService: OfficeService,
    public authService: AuthService,
    private fb: FormBuilder
  ) {
    this.patientForm = this.fb.group({
      officeId: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      phone: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      address: ['', Validators.required],
      caregiverIds: [[]]
    });
  }

  ngOnInit(): void {
    this.loadPatients();
    this.loadOffices();

    // Watch for office changes to reload caregivers
    this.patientForm.get('officeId')?.valueChanges.subscribe(officeId => {
      if (officeId) {
        this.loadCaregivers(officeId);
      }
    });
  }

  loadPatients(): void {
    this.loading = true;
    this.patientService.getPatients(this.page, this.pageSize, this.sortBy, this.sortDescending).subscribe({
      next: (result) => {
        this.patients = result.data;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load patients';
        this.loading = false;
      }
    });
  }

  sortColumn(column: string): void {
    if (this.sortBy === column) {
      this.sortDescending = !this.sortDescending;
    } else {
      this.sortBy = column;
      this.sortDescending = false;
    }
    this.page = 1; // Reset to first page when sorting
    this.loadPatients();
  }

  getSortIcon(column: string): string {
    if (this.sortBy !== column) {
      return '↕️';
    }
    return this.sortDescending ? '↓' : '↑';
  }

  loadCaregivers(officeId?: number): void {
    this.caregiverService.searchCaregivers('', 1, 100, 'FirstName', false, officeId).subscribe({
      next: (result) => {
        this.caregivers = result.data;
        // Clear selected caregivers when office changes
        if (officeId && !this.editMode) {
          this.patientForm.patchValue({ caregiverIds: [] });
        }
      },
      error: (err) => {
        console.error('Failed to load caregivers', err);
      }
    });
  }

  loadOffices(): void {
    this.officeService.searchOffices().subscribe({
      next: (result) => {
        this.offices = result.data;
        // Set first office as default if available
        if (this.offices.length > 0 && !this.editMode) {
          this.patientForm.patchValue({ officeId: this.offices[0].id });
        }
      },
      error: (err) => {
        console.error('Failed to load offices', err);
      }
    });
  }

  onSubmit(): void {
    if (this.patientForm.valid) {
      this.loading = true;
      const formValue = this.patientForm.value;

      const formData: CreatePatient = {
        ...formValue,
        caregiverIds: formValue.caregiverIds || []
      };

      if (this.editMode && this.editingPatientId) {
        this.patientService.updatePatient(this.editingPatientId, formData).subscribe({
          next: () => {
            this.loadPatients();
            this.closeModal(); // Close modal after successful update
            this.showSuccess('Patient updated successfully!');
            this.loading = false;
          },
          error: (err) => {
            this.error = 'Failed to update patient';
            this.loading = false;
          }
        });
      } else {
        this.patientService.createPatient(formData).subscribe({
          next: () => {
            this.loadPatients();
            this.resetForm();
            this.showSuccess('Patient created successfully!');
            this.loading = false;
          },
          error: (err) => {
            this.error = 'Failed to create patient';
            this.loading = false;
          }
        });
      }
    }
  }

  editPatient(patient: Patient): void {
    this.editMode = true;
    this.editingPatientId = patient.id;
    this.showForm = false; // Don't show the regular form, use modal instead
    this.patientForm.patchValue({
      officeId: patient.officeId,
      firstName: patient.firstName,
      lastName: patient.lastName,
      dateOfBirth: patient.dateOfBirth.split('T')[0],
      phone: patient.phone,
      email: patient.email,
      address: patient.address,
      caregiverIds: patient.caregivers.map(c => c.id)
    });
  }

  closeModal(): void {
    this.editMode = false;
    this.editingPatientId = null;
    this.error = '';
  }

  deletePatient(id: number): void {
    if (confirm('Are you sure you want to delete this patient?')) {
      this.patientService.deletePatient(id).subscribe({
        next: () => {
          this.loadPatients();
          this.showSuccess('Patient deleted successfully!');
        },
        error: (err) => {
          this.error = 'Failed to delete patient';
        }
      });
    }
  }

  toggleForm(): void {
    if (this.showForm) {
      // Hiding the form
      this.resetForm();
    } else {
      // Showing the form
      this.showForm = true;
      this.editMode = false;
      this.editingPatientId = null;
      this.patientForm.reset({ officeId: 1, caregiverIds: [] });
      this.error = '';
    }
  }

  resetForm(): void {
    const defaultOfficeId = this.offices.length > 0 ? this.offices[0].id : '';
    this.patientForm.reset({ officeId: defaultOfficeId, caregiverIds: [] });
    this.editMode = false;
    this.editingPatientId = null;
    this.showForm = false;
    this.error = '';
  }

  showSuccess(message: string): void {
    this.successMessage = message;
    this.error = '';
    // Auto-hide after 3 seconds
    setTimeout(() => {
      this.successMessage = '';
    }, 3000);
  }

  getCaregiverNames(caregivers: Caregiver[]): string {
    return caregivers.map(c => `${c.firstName} ${c.lastName}`).join(', ');
  }

  isCaregiverSelected(caregiverId: number): boolean {
    const selectedIds = this.patientForm.get('caregiverIds')?.value || [];
    return selectedIds.includes(caregiverId);
  }

  toggleCaregiver(caregiverId: number): void {
    const selectedIds = this.patientForm.get('caregiverIds')?.value || [];
    const index = selectedIds.indexOf(caregiverId);

    if (index > -1) {
      // Remove if already selected
      selectedIds.splice(index, 1);
    } else {
      // Add if not selected
      selectedIds.push(caregiverId);
    }

    this.patientForm.patchValue({ caregiverIds: selectedIds });
  }

  onPageChange(newPage: number): void {
    this.page = newPage;
    this.loadPatients();
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  getRowNumber(index: number): number {
    return (this.page - 1) * this.pageSize + index + 1;
  }

  getStartRecord(): number {
    return this.totalCount === 0 ? 0 : (this.page - 1) * this.pageSize + 1;
  }

  getEndRecord(): number {
    const end = this.page * this.pageSize;
    return end > this.totalCount ? this.totalCount : end;
  }
}
