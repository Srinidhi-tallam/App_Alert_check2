import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { Alarm } from '../models/alarm.model';
import { GuidIdDto } from '../models/dto/guid-id.dto';

@Injectable({ providedIn: 'root' })
export class AlarmService {
  private apiUrl = `${environment.apiUrl}/alarms`;

  constructor(private http: HttpClient) { }

  list(): Observable<Alarm[]> {
    return this.http.get<Alarm[]>(this.apiUrl);
  }

  ack(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/${id}/ack`, {});
  }

  ackFromBody(dto: GuidIdDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/ack`, dto);
  }
}
