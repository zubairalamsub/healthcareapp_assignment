export interface Caregiver {
  id: number;
  firstName: string;
  lastName: string;
  phone: string;
  email: string;
  specialization: string;
  officeName?: string;
}

export interface Patient {
  id: number;
  officeId: number;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  phone: string;
  email: string;
  address: string;
  caregivers: Caregiver[];
}

export interface CreatePatient {
  officeId: number;
  firstName: string;
  lastName: string;
  dateOfBirth: string;
  phone: string;
  email: string;
  address: string;
  caregiverIds?: number[];
}

export interface PagedResult<T> {
  data: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}
