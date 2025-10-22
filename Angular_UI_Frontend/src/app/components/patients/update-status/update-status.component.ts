import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PatientService } from '../../../services/patient.service';

@Component({
    selector: 'app-update-status',
    imports: [CommonModule, FormsModule],
    templateUrl: './update-status.component.html',
    styleUrls: ['./update-status.component.css']
})
export class UpdateStatusComponent {
  status = {
    isWalkingAlone: false,
    isFalling: false
  };

  constructor(private patientService: PatientService) { }

  saveStatus() {
    console.log('Status saved:', this.status);
    // Call API to save patient status
  }
}
