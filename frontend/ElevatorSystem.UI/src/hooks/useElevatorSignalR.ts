import { useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import { toast } from "react-toastify";
import type { Elevator } from "../types";
import { StorageUtil } from "../utils/storage.util";

interface UseElevatorSignalRReturn {
  elevators: Elevator[];
  connectionStatus: "connecting" | "connected" | "disconnected" | "error";
  logs: string[];
}

export const useElevatorSignalR = (
  buildingId: number
): UseElevatorSignalRReturn => {
  const [elevators, setElevators] = useState<Elevator[]>([]);
  const [connectionStatus, setConnectionStatus] = useState<
    "connecting" | "connected" | "disconnected" | "error"
  >("disconnected");
  const [logs, setLogs] = useState<string[]>([]);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connect = async () => {
      const token = StorageUtil.getToken();

      if (!token) {
        setConnectionStatus("error");
        setLogs((prev) => [
          `[${new Date().toLocaleTimeString()}] Error: Authentication token not found.`,
          ...prev,
        ]);
        toast.error("Authentication required to connect to elevator service.");
        return;
      }

      setConnectionStatus("connecting");
      setLogs((prev) => [
        `[${new Date().toLocaleTimeString()}] Attempting to connect...`,
        ...prev,
      ]);

      const hubUrl = `${import.meta.env.VITE_API_URL}/hubs/elevator`;

      connectionRef.current = new signalR.HubConnectionBuilder()
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

      connectionRef.current.on(
        "ReceiveElevatorUpdate",
        (elevator: Elevator) => {
          setElevators((prevElevators) => {
            const existingIndex = prevElevators.findIndex(
              (e) => e.id === elevator.id
            );
            if (existingIndex > -1) {
              // Update existing elevator
              const updatedElevators = [...prevElevators];
              updatedElevators[existingIndex] = elevator;
              return updatedElevators;
            } else {
              // Add new elevator
              return [...prevElevators, elevator];
            }
          });
          setLogs((prev) => [
            `[${new Date().toLocaleTimeString()}] Elevator ${
              elevator.id
            } updated: Floor ${elevator.currentFloor}, Status ${
              elevator.status
            }`,
            ...prev,
          ]);
        }
      );

      connectionRef.current.on("Connected", (message: string) => {
        setLogs((prev) => [
          `[${new Date().toLocaleTimeString()}] Server: ${message}`,
          ...prev,
        ]);
        toast.success(message);
      });

      connectionRef.current.on("JoinedGroup", (groupName: string) => {
        setLogs((prev) => [
          `[${new Date().toLocaleTimeString()}] Joined group: ${groupName}`,
          ...prev,
        ]);
        toast.info(`Joined building group ${groupName.split("_")[1]}`);
      });

      connectionRef.current.on("LeftGroup", (groupName: string) => {
        setLogs((prev) => [
          `[${new Date().toLocaleTimeString()}] Left group: ${groupName}`,
          ...prev,
        ]);
        toast.info(`Left building group ${groupName.split("_")[1]}`);
      });

      connectionRef.current.on("Error", (errorMessage: string) => {
        setLogs((prev) => [
          `[${new Date().toLocaleTimeString()}] Error: ${errorMessage}`,
          ...prev,
        ]);
        toast.error(`Hub Error: ${errorMessage}`);
      });

      connectionRef.current.onclose((error) => {
        if (error) {
          setConnectionStatus("error");
          setLogs((prev) => [
            `[${new Date().toLocaleTimeString()}] Connection closed with error: ${
              error.message
            }`,
            ...prev,
          ]);
          toast.error("SignalR connection closed due to an error.");
          console.error("SignalR connection closed with error:", error);
        } else {
          setConnectionStatus("disconnected");
          setLogs((prev) => [
            `[${new Date().toLocaleTimeString()}] Connection disconnected.`,
            ...prev,
          ]);
          toast.info("SignalR connection disconnected.");
        }
      });

      try {
        await connectionRef.current.start();
        setConnectionStatus("connected");
        setLogs((prev) => [
          `[${new Date().toLocaleTimeString()}] Connected to SignalR hub.`,
          ...prev,
        ]);
        await connectionRef.current.invoke("JoinBuildingGroup", buildingId);
        // eslint-disable-next-line @typescript-eslint/no-explicit-any
      } catch (err: any) {
        setConnectionStatus("error");
        setLogs((prev) => [
          `[${new Date().toLocaleTimeString()}] Failed to connect: ${
            err.message
          }`,
          ...prev,
        ]);
        toast.error(`Failed to connect to elevator hub: ${err.message}`);
        console.error("Failed to connect to SignalR:", err);
      }
    };

    const disconnect = async () => {
      if (
        connectionRef.current &&
        connectionRef.current.state !== signalR.HubConnectionState.Disconnected
      ) {
        try {
          await connectionRef.current.stop();
          setLogs((prev) => [
            `[${new Date().toLocaleTimeString()}] Connection stopped.`,
            ...prev,
          ]);
        } catch (err) {
          console.error("Error stopping SignalR connection:", err);
          setLogs((prev) => [
            `[${new Date().toLocaleTimeString()}] Error stopping connection.`,
            ...prev,
          ]);
        } finally {
          setConnectionStatus("disconnected");
        }
      }
    };

    connect();

    // Cleanup on component unmount or buildingId change
    return () => {
      disconnect();
    };
  }, [buildingId]);

  return { elevators, connectionStatus, logs };
};
