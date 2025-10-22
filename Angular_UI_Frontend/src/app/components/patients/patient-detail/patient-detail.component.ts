import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { PatientService } from '../../../services/patient.service';
import { Patient } from '../../../models/patient.model.js';
import { Alarm } from '../../../models/alarm.model.js';

@Component({
    selector: 'app-patient-detail',
    imports: [CommonModule],
    templateUrl: './patient-detail.component.html',
    styleUrls: ['./patient-detail.component.css']
})
export class PatientDetailComponent implements OnInit {
  patientId: string = '';
  patient?: Patient;
  alarms: Alarm[] = [];

  loadingAlarms: boolean = false;
  alarmsError: string | null = null;
  patientError: string | null = null;

  constructor(private route: ActivatedRoute, private patientService: PatientService) { }

  ngOnInit(): void {
    this.patientId = this.route.snapshot.paramMap.get('id') ?? '';
    if (this.patientId) {
      this.loadPatient();
      this.loadAlarms();
    }
  }

  loadPatient(): void {
    this.patientService.getById(this.patientId).subscribe({
      next: (p: Patient) => this.patient = p,
      error: (err) => {
        console.error(err);
        this.patientError = 'Failed to load patient details';
      }
    });
  }

  loadAlarms(): void {
    this.loadingAlarms = true;
    this.patientService.getAlarms(this.patientId).subscribe({
      next: (data: Alarm[]) => {
        this.alarms = data;
        this.loadingAlarms = false;
      },
      error: (err) => {
        console.error(err);
        this.alarmsError = 'Failed to load alarms';
        this.loadingAlarms = false;
      }
    });
  }
}



