import type { Elevator } from "../../types";

export interface UseElevatorSignalRReturn {
  elevators: Elevator[];
  connectionStatus: "connecting" | "connected" | "disconnected" | "error";
  logs: string[];
}

export interface ElevatorState {
  currentElevatorId: number | null;
  currentCallId: number | null;
  pendingCallId: number | null;
  pendingCallFloor: number | null;
  tabId: string;
}

export interface ReduxState {
  isInElevator: boolean;
  currentFloor: number;
}

export interface ElevatorAnalysis {
  isAwaitingDestinationNow: boolean;
  wasAwaitingDestinationBefore: boolean;
  destinationSelectionStateChanged: boolean;
  isElevatorAtUserFloor: boolean;
  isElevatorAtPendingCallFloor: boolean;
  isPendingCallMatch: boolean;
  serverBugWorkaround: boolean;
  userStartedOnSameFloor: boolean;
  justSelectedDestination: boolean;
  isPickupStop: boolean;
  shouldUserExit: boolean;
  isSameFloorPickup: boolean;
  isThisUserTheNewPickup: boolean;
  isThisUserAlreadyInElevator: boolean;
  isThisUserAlreadyTravelingToDestination: boolean;
  shouldUserStayInElevator: boolean;
  shouldPreventDestinationSelection: boolean;
}

export type ElevatorRef<T> = {
  current: T;
};
