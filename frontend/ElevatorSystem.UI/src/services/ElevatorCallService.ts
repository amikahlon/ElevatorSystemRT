import api from "./api";

export class ElevatorCallService {
  static async createElevatorCall(data: {
    buildingId: number;
    requestedFloor: number;
    direction: "up" | "down";
  }) {
    const directionEnumValue = data.direction === "up" ? "Up" : "Down";

    const response = await api.post("/api/elevatorcalls", {
      buildingId: data.buildingId,
      requestedFloor: data.requestedFloor,
      direction: directionEnumValue,
    });

    if (response?.data?.id) {
      const callData = {
        id: response.data.id,
        floor: data.requestedFloor,
        timestamp: Date.now(),
      };

      sessionStorage.setItem("pendingCallId", response.data.id.toString());
      sessionStorage.setItem(
        "pendingCallFloor",
        data.requestedFloor.toString()
      );
      sessionStorage.setItem("pendingCallData", JSON.stringify(callData));

      sessionStorage.setItem(
        "pendingElevatorCallId",
        response.data.id.toString()
      );
      sessionStorage.setItem(
        "pendingElevatorCallFloor",
        data.requestedFloor.toString()
      );
      sessionStorage.setItem(
        "pendingElevatorCallData",
        JSON.stringify(callData)
      );

      console.log(
        `[TabSession ${sessionStorage.getItem(
          "tabSessionId"
        )}] New elevator call created with ID: ${response.data.id} from floor ${
          data.requestedFloor
        }. Saved to sessionStorage with keys: pendingCallId, pendingCallFloor, pendingElevatorCallId, pendingElevatorCallFloor`
      );
    }

    return response;
  }

  static async selectDestination(callId: number, destinationFloor: number) {
    sessionStorage.setItem("currentElevatorCallId", callId.toString());

    return api.post(
      `/api/elevatorcalls/${callId}/destination`,
      destinationFloor
    );
  }
}
