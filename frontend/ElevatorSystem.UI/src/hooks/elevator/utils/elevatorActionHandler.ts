import type { Dispatch } from "@reduxjs/toolkit";
import { toast } from "react-toastify";
import type { Elevator } from "../../../types";
import type { ElevatorAnalysis, ReduxState, ElevatorRef } from "../types";
import {
  setCurrentFloor,
  setElevatorStatus,
  updateFloorDuringRide,
  enterElevatorAtCurrentFloor,
} from "../../../redux/slices/authSlice";
import { SessionStorageManager } from "./sessionStorageManager";

export class ElevatorActionHandler {
  static handleUserStayInElevator(
    elevator: Elevator,
    dispatch: Dispatch,
    reduxStateRef: ElevatorRef<ReduxState>,
    tabId: string
  ): void {
    console.log(
      `[TabSession ${tabId}] User staying in elevator ${elevator.id} during pickup stop - updating floor only. New callId=${elevator.currentCallId}, exitingCallId=${elevator.exitingCallId}`
    );

    if (reduxStateRef.current.currentFloor !== elevator.currentFloor) {
      console.log(
        `[TabSession ${tabId}] Floor changed during pickup stop: ${reduxStateRef.current.currentFloor} -> ${elevator.currentFloor}`
      );
      dispatch(setCurrentFloor(elevator.currentFloor));
      reduxStateRef.current.currentFloor = elevator.currentFloor;
    }
  }

  static handleUserEnterElevator(
    elevator: Elevator,
    analysis: ElevatorAnalysis,
    dispatch: Dispatch,
    reduxStateRef: ElevatorRef<ReduxState>,
    currentElevatorIdRef: ElevatorRef<number | null>,
    currentCallIdRef: ElevatorRef<number | null>,
    pendingCallIdRef: ElevatorRef<number | null>,
    pendingCallFloorRef: ElevatorRef<number | null>,
    tabId: string
  ): void {
    console.log(
      `[TabSession ${tabId}] User entering elevator ${elevator.id} at floor ${
        elevator.currentFloor
      } with callId ${elevator.currentCallId}, awaitingDestCallId=${
        elevator.awaitingDestinationCallId
      }${analysis.isPickupStop ? " (pickup stop)" : ""} ${
        elevator.currentFloor === reduxStateRef.current.currentFloor
          ? " (user already at same floor)"
          : ""
      }${
        !analysis.wasAwaitingDestinationBefore &&
        elevator.isAwaitingDestinationSelection
          ? " (first time awaiting destination)"
          : ""
      }${
        elevator.awaitingDestinationCallId === pendingCallIdRef.current ||
        elevator.awaitingDestinationCallId === currentCallIdRef.current
          ? " (this user needs to select destination)"
          : ""
      }`
    );

    if (analysis.isSameFloorPickup) {
      console.log(
        `[TabSession ${tabId}] Same floor pickup detected - user already at correct floor ${elevator.currentFloor}`
      );
      dispatch(enterElevatorAtCurrentFloor());
    } else {
      if (reduxStateRef.current.currentFloor !== elevator.currentFloor) {
        console.log(
          `[TabSession ${tabId}] Updating user floor from ${reduxStateRef.current.currentFloor} to ${elevator.currentFloor} before entering elevator`
        );
        dispatch(setCurrentFloor(elevator.currentFloor));
        reduxStateRef.current.currentFloor = elevator.currentFloor;
      }
      dispatch(setElevatorStatus(true));
    }

    // Update refs and session storage
    currentElevatorIdRef.current = elevator.id;
    currentCallIdRef.current = elevator.currentCallId;

    SessionStorageManager.updateCurrentElevatorCall(
      elevator.id,
      elevator.currentCallId
    );

    if (pendingCallIdRef.current === elevator.currentCallId) {
      SessionStorageManager.clearPendingCall();
      pendingCallIdRef.current = null;
      pendingCallFloorRef.current = null;
    }

    reduxStateRef.current.currentFloor = elevator.currentFloor;
    reduxStateRef.current.isInElevator = true;

    toast.info(`Elevator arrived! Please select your destination.`);

    console.log(
      `[TabSession ${tabId}] Successfully entered elevator. New state: isInElevator=${reduxStateRef.current.isInElevator}, currentFloor=${reduxStateRef.current.currentFloor}, elevatorId=${currentElevatorIdRef.current}, callId=${currentCallIdRef.current}`
    );
  }

  static handleFloorUpdate(
    elevator: Elevator,
    prevElevator: Elevator,
    dispatch: Dispatch,
    reduxStateRef: ElevatorRef<ReduxState>,
    currentCallIdRef: ElevatorRef<number | null>,
    tabId: string
  ): void {
    if (elevator.status === 1) {
      console.log(
        `[TabSession ${tabId}] Floor update during ride: ${prevElevator.currentFloor} -> ${elevator.currentFloor} (callId: ${currentCallIdRef.current})`
      );
      dispatch(updateFloorDuringRide(elevator.currentFloor));
    } else {
      console.log(
        `[TabSession ${tabId}] Floor sync update: Redux floor ${reduxStateRef.current.currentFloor} -> Elevator floor ${elevator.currentFloor} (status: ${elevator.status}, callId: ${currentCallIdRef.current})`
      );
      dispatch(setCurrentFloor(elevator.currentFloor));
    }

    reduxStateRef.current.currentFloor = elevator.currentFloor;
  }

  static handleUserExitElevator(
    elevator: Elevator,
    dispatch: Dispatch,
    reduxStateRef: ElevatorRef<ReduxState>,
    currentElevatorIdRef: ElevatorRef<number | null>,
    currentCallIdRef: ElevatorRef<number | null>,
    tabId: string
  ): void {
    console.log(
      `[TabSession ${tabId}] User exiting elevator ${elevator.id} at floor ${elevator.currentFloor} - exitingCallId=${elevator.exitingCallId} matches my callId=${currentCallIdRef.current}`
    );

    currentElevatorIdRef.current = null;
    currentCallIdRef.current = null;
    SessionStorageManager.clearElevatorState();

    if (reduxStateRef.current.isInElevator) {
      dispatch(setElevatorStatus(false));
    }
    if (reduxStateRef.current.currentFloor !== elevator.currentFloor) {
      dispatch(setCurrentFloor(elevator.currentFloor));
    }

    reduxStateRef.current.currentFloor = elevator.currentFloor;
    reduxStateRef.current.isInElevator = false;

    toast.success(`Arrived at floor ${elevator.currentFloor}!`);
  }

  static handleUserArrivedAtDestination(
    floorNumber: number,
    dispatch: Dispatch,
    currentElevatorIdRef: ElevatorRef<number | null>,
    currentCallIdRef: ElevatorRef<number | null>,
    reduxStateRef: ElevatorRef<ReduxState>,
    setLogs: React.Dispatch<React.SetStateAction<string[]>>,
    tabId: string
  ): void {
    if (
      reduxStateRef.current.isInElevator &&
      currentElevatorIdRef.current !== null
    ) {
      console.log(
        `[TabSession ${tabId}] Server confirmed arrival at floor ${floorNumber}, clearing elevator state`
      );

      currentElevatorIdRef.current = null;
      currentCallIdRef.current = null;
      SessionStorageManager.clearElevatorState();

      dispatch(setCurrentFloor(floorNumber));
      dispatch(setElevatorStatus(false));

      setLogs((prev) => [
        `[${new Date().toLocaleTimeString()}] Server confirmed: You have arrived at floor ${floorNumber}`,
        ...prev,
      ]);
      toast.success(`You have arrived at floor ${floorNumber}`);
    } else {
      console.log(
        `[TabSession ${tabId}] Ignoring server arrival notification - user not in elevator in this tab`
      );
    }
  }

  static logPreventionReasons(
    elevator: Elevator,
    analysis: ElevatorAnalysis,
    currentCallIdRef: ElevatorRef<number | null>,
    pendingCallIdRef: ElevatorRef<number | null>,
    currentElevatorIdRef: ElevatorRef<number | null>,
    reduxState: ReduxState,
    tabId: string
  ): void {
    if (
      !analysis.shouldPreventDestinationSelection &&
      elevator.isAwaitingDestinationSelection &&
      elevator.currentCallId &&
      !reduxState.isInElevator &&
      (pendingCallIdRef.current === elevator.currentCallId ||
        currentCallIdRef.current === elevator.currentCallId)
    ) {
      console.log(
        `[TabSession ${tabId}] NOT ENTERING ELEVATOR: isThisUserAlreadyInElevator=${
          analysis.isThisUserAlreadyInElevator
        }, isInElevator=${
          reduxState.isInElevator
        }, hasCurrentElevator=${!!currentElevatorIdRef.current}, elevatorCallId=${
          elevator.currentCallId
        }, pendingCallMatch=${
          pendingCallIdRef.current === elevator.currentCallId
        }, currentCallMatch=${
          currentCallIdRef.current === elevator.currentCallId
        }, awaiting=${
          elevator.isAwaitingDestinationSelection
        }, isThisUserTheNewPickup=${analysis.isThisUserTheNewPickup}`
      );
    }

    if (
      analysis.isThisUserTheNewPickup &&
      currentElevatorIdRef.current !== null
    ) {
      console.log(
        `[TabSession ${tabId}] PREVENTED DUPLICATE ENTRY: User already has assigned elevator ${
          currentElevatorIdRef.current
        }, not entering elevator ${elevator.id} again. CallId=${
          elevator.currentCallId
        }, myCallId=${currentCallIdRef.current}, sameFloorCase=${
          elevator.currentFloor === reduxState.currentFloor
        }`
      );
    }

    this.logAdditionalPreventions(
      elevator,
      analysis,
      currentCallIdRef,
      reduxState,
      tabId
    );
  }

  private static logAdditionalPreventions(
    elevator: Elevator,
    analysis: ElevatorAnalysis,
    currentCallIdRef: ElevatorRef<number | null>,
    reduxState: ReduxState,
    tabId: string
  ): void {
    if (
      reduxState.isInElevator &&
      elevator.id === currentCallIdRef.current &&
      elevator.isAwaitingDestinationSelection &&
      elevator.currentFloor === reduxState.currentFloor
    ) {
      console.log(
        `[TabSession ${tabId}] SAME FLOOR BUG DETECTED: User already in elevator ${elevator.id} at floor ${elevator.currentFloor}, but elevator awaiting destination again! CallId=${elevator.currentCallId}, myCallId=${currentCallIdRef.current}`
      );
    }

    if (
      analysis.isThisUserAlreadyInElevator &&
      elevator.isAwaitingDestinationSelection &&
      ((elevator.awaitingDestinationCallId &&
        elevator.awaitingDestinationCallId !== currentCallIdRef.current) ||
        (!elevator.awaitingDestinationCallId &&
          elevator.currentCallId !== currentCallIdRef.current))
    ) {
      console.log(
        `[TabSession ${tabId}] PREVENTED: User already in elevator ${elevator.id} but server asking for destination selection again. My callId=${currentCallIdRef.current}, elevator callId=${elevator.currentCallId}, awaitingDestCallId=${elevator.awaitingDestinationCallId}. This destination selection is for someone else!`
      );
    }
  }
}
