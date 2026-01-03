import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { PatientComponent } from './components/patient/patient.component';
import { CaregiverComponent } from './components/caregiver/caregiver.component';
import { OfficeComponent } from './components/office/office.component';
import { AuthGuard } from './guards/auth.guard';

const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'patients', component: PatientComponent, canActivate: [AuthGuard] },
  { path: 'caregivers', component: CaregiverComponent, canActivate: [AuthGuard] },
  { path: 'offices', component: OfficeComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/patients', pathMatch: 'full' },
  { path: '**', redirectTo: '/patients' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
