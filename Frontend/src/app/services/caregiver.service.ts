import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Caregiver, PagedResult } from '../models/patient.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CaregiverService {
  private apiUrl = `${environment.apiUrl}/caregivers`;

  constructor(private http: HttpClient) {}

  searchCaregivers(keyword: string = '', page: number = 1, pageSize: number = 50, sortBy: string = 'FirstName', sortDescending: boolean = false, officeId?: number): Observable<PagedResult<Caregiver>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    if (keyword && keyword.trim()) {
      params = params.set('keyword', keyword.trim());
    }

    if (officeId) {
      params = params.set('officeId', officeId.toString());
    }

    if (sortBy) {
      params = params.set('sortBy', sortBy);
    }

    params = params.set('sortDescending', sortDescending.toString());

    return this.http.get<PagedResult<Caregiver>>(`${this.apiUrl}/search`, { params });
  }
}
