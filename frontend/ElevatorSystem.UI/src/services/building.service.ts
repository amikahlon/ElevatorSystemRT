import api from "./api";
import type { Building, Elevator } from "../types";

export class BuildingService {
  static async getMyBuildings(): Promise<Building[]> {
    const res = await api.get<Building[]>("/api/buildings/my");
    return res.data;
  }

  static async getElevatorsByBuilding(buildingId: number): Promise<Elevator[]> {
    const res = await api.get<Elevator[]>(
      `/api/elevators/by-building/${buildingId}`
    );
    return res.data;
  }

  static async createBuilding(data: {
    name: string;
    numberOfFloors: number;
  }): Promise<Building> {
    const res = await api.post<Building>("/api/buildings", data);
    return res.data;
  }
}
