import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
//import { Alarm } from '../../../models/alarm.model';
import { Alarm } from '../../../models/alarm.model.js'; 

@Component({
    selector: 'app-alarm-detail',
    imports: [CommonModule],
    templateUrl: './alarm-detail.component.html',
    styleUrls: ['./alarm-detail.component.css']
})
export class AlarmDetailComponent {
  @Input() alarm!: Alarm;
}
