import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { CaregiverService } from '../../services/caregiver.service';
import { OfficeService, Office } from '../../services/office.service';
import { Caregiver, PagedResult } from '../../models/patient.model';

@Component({
  selector: 'app-caregiver',
  templateUrl: './caregiver.component.html',
  styleUrls: ['./caregiver.component.css']
})
export class CaregiverComponent implements OnInit {
  caregivers: Caregiver[] = [];
  offices: Office[] = [];
  searchForm: FormGroup;
  totalCount = 0;
  page = 1;
  pageSize = 10;
  loading = false;
  error = '';

  constructor(
    private caregiverService: CaregiverService,
    private officeService: OfficeService,
    private fb: FormBuilder
  ) {
    this.searchForm = this.fb.group({
      keyword: [''],
      officeId: [null],
      sortBy: ['FirstName'],
      sortDescending: [false]
    });
  }

  ngOnInit(): void {
    this.loadOffices();
    this.loadCaregivers();
  }

  loadCaregivers(): void {
    this.loading = true;
    const formValue = this.searchForm.value;

    this.caregiverService.searchCaregivers(
      formValue.keyword,
      this.page,
      this.pageSize,
      formValue.sortBy,
      formValue.sortDescending,
      formValue.officeId
    ).subscribe({
      next: (result: PagedResult<Caregiver>) => {
        this.caregivers = result.data;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load caregivers';
        this.loading = false;
      }
    });
  }

  loadOffices(): void {
    this.officeService.searchOffices({ pageSize: 100 }).subscribe({
      next: (result: PagedResult<Office>) => {
        this.offices = result.data;
      },
      error: (err) => {
        console.error('Failed to load offices', err);
      }
    });
  }

  sortColumn(column: string): void {
    const currentSort = this.searchForm.get('sortBy')?.value;
    if (currentSort === column) {
      const currentDesc = this.searchForm.get('sortDescending')?.value;
      this.searchForm.patchValue({ sortDescending: !currentDesc });
    } else {
      this.searchForm.patchValue({
        sortBy: column,
        sortDescending: false
      });
    }
    this.page = 1;
    this.loadCaregivers();
  }

  getSortIcon(column: string): string {
    const sortBy = this.searchForm.get('sortBy')?.value;
    const sortDesc = this.searchForm.get('sortDescending')?.value;
    if (sortBy !== column) {
      return '↕️';
    }
    return sortDesc ? '↓' : '↑';
  }

  onSearch(): void {
    this.page = 1;
    this.loadCaregivers();
  }

  onPageChange(newPage: number): void {
    this.page = newPage;
    this.loadCaregivers();
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }
}
