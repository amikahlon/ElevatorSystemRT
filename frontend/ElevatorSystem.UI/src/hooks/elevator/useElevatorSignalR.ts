import { useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import type { Elevator } from "../../types";
import { useAppDispatch, useAppSelector } from "../../redux/hooks";
import { setElevatorStatus } from "../../redux/slices/authSlice";
import type {
  UseElevatorSignalRReturn,
  ElevatorState,
  ReduxState,
} from "./types";
import {
  SessionStorageManager,
  SignalRConnectionManager,
  StorageEventManager,
} from "./utils";

export const useElevatorSignalR = (
  buildingId: number
): UseElevatorSignalRReturn => {
  // State
  const [elevators, setElevators] = useState<Elevator[]>([]);
  const [connectionStatus, setConnectionStatus] = useState<
    "connecting" | "connected" | "disconnected" | "error"
  >("disconnected");
  const [logs, setLogs] = useState<string[]>([]);

  // refs!!!
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const dispatch = useAppDispatch();

  // Redux state
  const reduxIsInElevator = useAppSelector((state) => state.auth.isInElevator);
  const reduxCurrentFloor = useAppSelector((state) => state.auth.currentFloor);

  // Initialize state from session storage
  const elevatorState: ElevatorState =
    SessionStorageManager.initializeElevatorState();

  // create refs from initialized state
  const tabIdRef = useRef<string>(elevatorState.tabId);
  const currentCallIdRef = useRef<number | null>(elevatorState.currentCallId);
  const currentElevatorIdRef = useRef<number | null>(
    elevatorState.currentElevatorId
  );
  const pendingCallIdRef = useRef<number | null>(elevatorState.pendingCallId);
  const pendingCallFloorRef = useRef<number | null>(
    elevatorState.pendingCallFloor
  );

  // Redux state ref for event handlers
  const reduxStateRef = useRef<ReduxState>({
    isInElevator: reduxIsInElevator,
    currentFloor: reduxCurrentFloor,
  });

  // keep redux state ref updated !
  reduxStateRef.current = {
    isInElevator: reduxIsInElevator,
    currentFloor: reduxCurrentFloor,
  };

  // Setup storage event listeners
  useEffect(() => {
    const cleanupStorageEvents = StorageEventManager.setupStorageEventListeners(
      currentCallIdRef,
      pendingCallIdRef,
      pendingCallFloorRef,
      reduxStateRef,
      tabIdRef
    );

    return cleanupStorageEvents;
  }, []);

  // setup SignalR connection
  useEffect(() => {
    let cleanupConnection: (() => void) | null = null;

    const initializeConnection = async () => {
      const connection = await SignalRConnectionManager.createConnection(
        buildingId,
        reduxStateRef,
        currentCallIdRef,
        currentElevatorIdRef,
        pendingCallIdRef,
        pendingCallFloorRef,
        tabIdRef,
        dispatch,
        setElevators,
        setConnectionStatus,
        setLogs
      );

      if (connection) {
        connectionRef.current = connection;

        cleanupConnection = () => {
          SignalRConnectionManager.cleanupConnection(
            connection,
            setConnectionStatus,
            setLogs
          );

          // Clear local state
          dispatch(setElevatorStatus(false));
          currentElevatorIdRef.current = null;
          currentCallIdRef.current = null;
          pendingCallIdRef.current = null;
          pendingCallFloorRef.current = null;
          SessionStorageManager.clearAllElevatorData();
        };
      }
    };

    initializeConnection();

    return () => {
      if (cleanupConnection) {
        cleanupConnection();
      }
    };
  }, [buildingId, dispatch]);

  return { elevators, connectionStatus, logs };
};
