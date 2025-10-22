export interface CreatePatientDto {
  name: string;
  age: number;
  location?: {
    latitude?: number;
    longitude?: number;
  };
}
