import api from "./api";

export class ElevatorCallService {
  static async createElevatorCall(data: {
    buildingId: number;
    requestedFloor: number;
    direction: "up" | "down";
  }) {
    const directionEnumValue = data.direction === "up" ? "Up" : "Down";

    return api.post("/api/elevatorcalls", {
      buildingId: data.buildingId,
      requestedFloor: data.requestedFloor,
      direction: directionEnumValue,
    });
  }
}
