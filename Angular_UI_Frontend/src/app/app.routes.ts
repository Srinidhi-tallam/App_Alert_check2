import { Routes } from '@angular/router';
import { PatientListComponent } from './components/patients/patient-list/patient-list.component';
import { PatientDetailComponent } from './components/patients/patient-detail/patient-detail.component';
import { AlarmListComponent } from './components/alarms/alarm-list/alarm-list.component';
import { AlarmDetailComponent } from './components/alarms/alarm-detail/alarm-detail.component';
import { UpdateStatusComponent } from './components/patients/update-status/update-status.component';

export const routes: Routes = [
  { path: '', redirectTo: 'patients', pathMatch: 'full' },
  { path: 'patients', component: PatientListComponent },
  { path: 'patients/:id', component: PatientDetailComponent },
  { path: 'patients/:id/update-status', component: UpdateStatusComponent },
  { path: 'alarms', component: AlarmListComponent },
  { path: 'alarms/:id', component: AlarmDetailComponent },
];
