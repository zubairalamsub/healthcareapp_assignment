import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedResult } from '../models/patient.model';

export interface Office {
  id: number;
  name: string;
  address: string;
  phone: string;
  patientCount: number;
  caregiverCount: number;
}

export interface OfficeSearchParams {
  keyword?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDescending?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class OfficeService {
  private apiUrl = `${environment.apiUrl}/offices`;

  constructor(private http: HttpClient) {}

  searchOffices(params: OfficeSearchParams = {}): Observable<PagedResult<Office>> {
    let httpParams = new HttpParams()
      .set('page', (params.page || 1).toString())
      .set('pageSize', (params.pageSize || 10).toString())
      .set('sortBy', params.sortBy || 'Name')
      .set('sortDescending', (params.sortDescending || false).toString());

    if (params.keyword) {
      httpParams = httpParams.set('keyword', params.keyword);
    }

    return this.http.get<PagedResult<Office>>(`${this.apiUrl}/search`, { params: httpParams });
  }

  getOfficeById(id: number): Observable<Office> {
    return this.http.get<Office>(`${this.apiUrl}/${id}`);
  }
}
