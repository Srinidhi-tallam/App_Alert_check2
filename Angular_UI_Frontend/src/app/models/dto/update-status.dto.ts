export interface UpdateStatusDto {
  isWalkingAlone: boolean;
  isFalling: boolean;
  location?: {
    latitude?: number;
    longitude?: number;
  };
  notes?: string;
}
