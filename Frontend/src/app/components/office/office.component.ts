import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { OfficeService, Office } from '../../services/office.service';
import { PagedResult } from '../../models/patient.model';

@Component({
  selector: 'app-office',
  templateUrl: './office.component.html',
  styleUrls: ['./office.component.css']
})
export class OfficeComponent implements OnInit {
  offices: Office[] = [];
  searchForm: FormGroup;
  totalCount = 0;
  page = 1;
  pageSize = 10;
  loading = false;
  error = '';

  constructor(
    private officeService: OfficeService,
    private fb: FormBuilder
  ) {
    this.searchForm = this.fb.group({
      keyword: [''],
      sortBy: ['Name'],
      sortDescending: [false]
    });
  }

  ngOnInit(): void {
    this.loadOffices();
  }

  loadOffices(): void {
    this.loading = true;
    const formValue = this.searchForm.value;

    this.officeService.searchOffices({
      keyword: formValue.keyword,
      page: this.page,
      pageSize: this.pageSize,
      sortBy: formValue.sortBy,
      sortDescending: formValue.sortDescending
    }).subscribe({
      next: (result: PagedResult<Office>) => {
        this.offices = result.data;
        this.totalCount = result.totalCount;
        this.loading = false;
      },
      error: (err) => {
        this.error = 'Failed to load offices';
        this.loading = false;
      }
    });
  }

  onSearch(): void {
    this.page = 1;
    this.loadOffices();
  }

  onPageChange(newPage: number): void {
    this.page = newPage;
    this.loadOffices();
  }

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }
}
