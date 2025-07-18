export interface Building {
  id: number;
  name: string;
  numberOfFloors: number;
  createdAt: string;
  updatedAt?: string;
}

export interface Elevator {
  id: number;
  buildingId: number;
  currentFloor: number;
  status: number;
  direction: number;
  doorStatus: number;
  createdAt: string;
  updatedAt?: string;
}
