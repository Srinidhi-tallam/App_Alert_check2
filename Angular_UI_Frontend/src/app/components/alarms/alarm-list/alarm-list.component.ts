import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PatientService } from '../../../services/patient.service';
import { Alarm } from '../../../models/alarm.model.js';

@Component({
    selector: 'app-alarm-list',
    imports: [CommonModule],
    templateUrl: './alarm-list.component.html',
    styleUrls: ['./alarm-list.component.css']
})
export class AlarmListComponent implements OnInit {
  alarms: Alarm[] = [];
  errorMessage: string | null = null;

  constructor(private patientService: PatientService) { }

  ngOnInit(): void {
    // Example: fetch all alarms for patientId '123' or adapt as needed
    this.patientService.getAlarms('123').subscribe({
      next: (data) => this.alarms = data,
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Failed to load alarms';
      }
    });
  }

  acknowledge(id: string | undefined): void {
    if (!id) return;
    // Call your API to acknowledge alarm
    console.log('Acknowledged alarm id:', id);
  }
}
