export interface Alarm {
  id: string;
  name: string;
  severity: string;
  timestamp?: string;        // for patient-detail component
  createdAt?: string;        // for alarm-list/detail components
  message?: string;          // for alarm-list component
  patientName?: string;      // for alarm-list component
  isAcknowledged?: boolean;  // for alarm-list/detail component
}


