import type { Dispatch } from "@reduxjs/toolkit";
import * as signalR from "@microsoft/signalr";
import { toast } from "react-toastify";
import type { Elevator } from "../../../types";
import type { ReduxState, ElevatorRef } from "../types";
import { StorageUtil } from "../../../utils/storage.util";
import { SignalREventHandlers } from "./signalREventHandlers";

export class SignalRConnectionManager {
  static async createConnection(
    buildingId: number,
    reduxStateRef: ElevatorRef<ReduxState>,
    currentCallIdRef: ElevatorRef<number | null>,
    currentElevatorIdRef: ElevatorRef<number | null>,
    pendingCallIdRef: ElevatorRef<number | null>,
    pendingCallFloorRef: ElevatorRef<number | null>,
    tabIdRef: ElevatorRef<string>,
    dispatch: Dispatch,
    setElevators: React.Dispatch<React.SetStateAction<Elevator[]>>,
    setConnectionStatus: React.Dispatch<
      React.SetStateAction<
        "connecting" | "connected" | "disconnected" | "error"
      >
    >,
    setLogs: React.Dispatch<React.SetStateAction<string[]>>
  ): Promise<signalR.HubConnection | null> {
    const token = StorageUtil.getToken();
    if (!token) {
      setConnectionStatus("error");
      setLogs((prev) => [
        `[${new Date().toLocaleTimeString()}] Error: Authentication token not found.`,
        ...prev,
      ]);
      toast.error("Authentication required to connect to elevator service.");
      return null;
    }

    setConnectionStatus("connecting");
    setLogs((prev) => [
      `[${new Date().toLocaleTimeString()}] Attempting to connect...`,
      ...prev,
    ]);

    const hubUrl = `${import.meta.env.VITE_API_URL}/hubs/elevator`;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: (retryContext) => {
          setLogs((prev) => [
            `[${new Date().toLocaleTimeString()}] Reconnecting... (Attempt ${
              retryContext.elapsedMilliseconds / 1000
            }s since disconnect)`,
            ...prev,
          ]);
          return Math.min(retryContext.elapsedMilliseconds * 2, 30000);
        },
      })
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.setupEventHandlers(
      connection,
      reduxStateRef,
      currentCallIdRef,
      currentElevatorIdRef,
      pendingCallIdRef,
      pendingCallFloorRef,
      tabIdRef,
      dispatch,
      setElevators,
      setLogs
    );

    try {
      await connection.start();
      await connection.invoke("JoinBuildingGroup", buildingId);
      setConnectionStatus("connected");
      setLogs((prev) => [
        `[${new Date().toLocaleTimeString()}] Connected to SignalR Hub.`,
        ...prev,
      ]);
      return connection;
    } catch (error) {
      console.error("SignalR Connection Error:", error);
      setConnectionStatus("error");
      setLogs((prev) => [
        `[${new Date().toLocaleTimeString()}] Connection failed: ${
          error instanceof Error ? error.message : String(error)
        }`,
        ...prev,
      ]);
      toast.error(`Could not connect to elevator service: ${error}`);
      return null;
    }
  }

  private static setupEventHandlers(
    connection: signalR.HubConnection,
    reduxStateRef: ElevatorRef<ReduxState>,
    currentCallIdRef: ElevatorRef<number | null>,
    currentElevatorIdRef: ElevatorRef<number | null>,
    pendingCallIdRef: ElevatorRef<number | null>,
    pendingCallFloorRef: ElevatorRef<number | null>,
    tabIdRef: ElevatorRef<string>,
    dispatch: Dispatch,
    setElevators: React.Dispatch<React.SetStateAction<Elevator[]>>,
    setLogs: React.Dispatch<React.SetStateAction<string[]>>
  ): void {
    // Main elevator update handler!
    connection.on(
      "ReceiveElevatorUpdate",
      SignalREventHandlers.createReceiveElevatorUpdateHandler(
        reduxStateRef,
        currentCallIdRef,
        currentElevatorIdRef,
        pendingCallIdRef,
        pendingCallFloorRef,
        tabIdRef,
        dispatch,
        setElevators,
        setLogs
      )
    );

    // Connection status handlers
    connection.on(
      "Connected",
      SignalREventHandlers.createConnectedHandler(setLogs)
    );
    connection.on(
      "JoinedGroup",
      SignalREventHandlers.createJoinedGroupHandler(setLogs)
    );
    connection.on("Error", SignalREventHandlers.createErrorHandler(setLogs));

    // User action handlers
    connection.on(
      "UserArrivedAtDestination",
      SignalREventHandlers.createUserArrivedAtDestinationHandler(
        reduxStateRef,
        currentElevatorIdRef,
        currentCallIdRef,
        dispatch,
        setLogs,
        tabIdRef
      )
    );

    // Elevator call handlers
    connection.on(
      "ElevatorCallAccepted",
      SignalREventHandlers.createElevatorCallAcceptedHandler(setLogs)
    );
    connection.on(
      "ElevatorCalled",
      SignalREventHandlers.createElevatorCalledHandler(setLogs)
    );
  }

  static cleanupConnection(
    connection: signalR.HubConnection | null,
    setConnectionStatus: React.Dispatch<
      React.SetStateAction<
        "connecting" | "connected" | "disconnected" | "error"
      >
    >,
    setLogs: React.Dispatch<React.SetStateAction<string[]>>
  ): void {
    if (connection) {
      connection.stop();
      setConnectionStatus("disconnected");
      setLogs((prev) => [
        `[${new Date().toLocaleTimeString()}] Disconnected from SignalR Hub.`,
        ...prev,
      ]);
    }
  }
}
