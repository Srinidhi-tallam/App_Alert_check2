import { Alarm } from './alarm.model';

export interface Patient {
  id: string;
  name: string;
  age: number;
  location?: {
    latitude?: number;
    longitude?: number;
  };
  lastUpdated?: string;
  alarms?: Alarm[];
}
