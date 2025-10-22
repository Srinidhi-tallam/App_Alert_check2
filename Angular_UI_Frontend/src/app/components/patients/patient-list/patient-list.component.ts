import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PatientService } from '../../../services/patient.service';
import { Patient } from '../../../models/patient.model';
import { RouterModule } from '@angular/router';

interface PatientStatus {
  isFalling: boolean;
  isWalkingAlone: boolean;
}

interface Location {
  latitude?: number;
  longitude?: number;
  name?: string;
}

@Component({
    selector: 'app-patient-list',
    imports: [CommonModule, RouterModule, FormsModule],
    templateUrl: './patient-list.component.html',
    styleUrls: ['./patient-list.component.css']
})
export class PatientListComponent implements OnInit {
  patients: Patient[] = [];
  newPatient: Partial<Patient> = { name: '', age: 0 };
  errorMessage: string | null = null;

  // Track status per patient
  patientStatus: { [id: string]: PatientStatus } = {};

  // Store alarms per patient
  patientAlarms: { [id: string]: any[] } = {};

  constructor(private patientService: PatientService) { }

  ngOnInit(): void {
    this.loadPatients();
  }

  loadPatients(): void {
    this.patientService.getAll().subscribe({
      next: (data) => {
        this.patients = data;
        this.errorMessage = data.length === 0 ? 'No patients found.' : null;

        data.forEach(p => {
          this.patientStatus[p.id || ''] = { isFalling: false, isWalkingAlone: false };
          this.loadAlarms(p.id || '');
        });
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to load patients';
      }
    });
  }

  createPatient(): void {
    if (!this.newPatient.name || !this.newPatient.age) return;

    this.patientService.create(this.newPatient as Patient).subscribe({
      next: (p) => {
        console.log('Patient created:', p);
        this.patients.push(p);
        this.newPatient = { name: '', age: 0 };
        this.errorMessage = null;
        this.patientStatus[p.id || ''] = { isFalling: false, isWalkingAlone: false };
        this.patientAlarms[p.id || ''] = [];
      },
      error: (err) => {
        console.error('Error creating patient:', err);
        this.errorMessage = 'Failed to create patient';
      }
    });
  }

  updateStatus(patientId: string): void {
    const status = this.patientStatus[patientId];
    if (!status) return;

    // Send a valid DTO matching backend
    const dto = {
      IsFalling: status.isFalling,
      IsWalkingAlone: status.isWalkingAlone,
      Location: null,  // Backend expects null if no location
      Notes: 'Updated from UI'
    };

    this.patientService.updateStatus(patientId, dto).subscribe({
      next: (alarm) => {
        console.log('Alarm created:', alarm);
        this.loadAlarms(patientId); // Refresh alarms
      },
      error: (err) => {
        console.error('Failed to update status:', err);
        this.errorMessage = 'Failed to update patient status';
      }
    });
  }

  loadAlarms(patientId: string): void {
    this.patientService.getAlarms(patientId).subscribe({
      next: (alarms) => this.patientAlarms[patientId] = alarms,
      error: (err) => console.error('Failed to load alarms:', err)
    });
  }
}

