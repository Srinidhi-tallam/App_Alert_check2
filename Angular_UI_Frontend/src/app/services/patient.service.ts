import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { Patient } from '../models/patient.model';
import { Alarm } from '../models/alarm.model';

@Injectable({
  providedIn: 'root'
})
export class PatientService {
  private apiUrl = `${environment.apiUrl}/patients`;

  constructor(private http: HttpClient) { }

  // GET all patients
  getAll(): Observable<Patient[]> {
    return this.http.get<Patient[]>(this.apiUrl);
  }

  // GET one patient by ID
  getById(id: string): Observable<Patient> {
    return this.http.get<Patient>(`${this.apiUrl}/${id}`);
  }

  // CREATE patient
  create(patient: Patient): Observable<Patient> {
    return this.http.post<Patient>(this.apiUrl, patient);
  }

  // UPDATE patient status
  updateStatus(id: string, statusData: any): Observable<Alarm[]> {
    return this.http.post<Alarm[]>(`${this.apiUrl}/${id}/status`, statusData);
  }

  // GET alarms for a specific patient
  getAlarms(id: string): Observable<Alarm[]> {
    return this.http.get<Alarm[]>(`${this.apiUrl}/${id}/alarms`);
  }
}
