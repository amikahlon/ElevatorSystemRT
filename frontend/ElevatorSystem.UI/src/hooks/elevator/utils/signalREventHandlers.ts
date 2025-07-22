import type { Dispatch } from "@reduxjs/toolkit";
import { toast } from "react-toastify";
import type { Elevator } from "../../../types";
import type { ReduxState, ElevatorAnalysis, ElevatorRef } from "../types";
import { ElevatorAnalyzer } from "./elevatorAnalyzer";
import { ElevatorActionHandler } from "./elevatorActionHandler";

export class SignalREventHandlers {
  static createReceiveElevatorUpdateHandler(
    reduxStateRef: ElevatorRef<ReduxState>,
    currentCallIdRef: ElevatorRef<number | null>,
    currentElevatorIdRef: ElevatorRef<number | null>,
    pendingCallIdRef: ElevatorRef<number | null>,
    pendingCallFloorRef: ElevatorRef<number | null>,
    tabIdRef: ElevatorRef<string>,
    dispatch: Dispatch,
    setElevators: React.Dispatch<React.SetStateAction<Elevator[]>>,
    setLogs: React.Dispatch<React.SetStateAction<string[]>>
  ) {
    return (elevator: Elevator) => {
      console.log(
        `[TabSession ${tabIdRef.current}] Elevator ${elevator.id} update: Floor=${elevator.currentFloor}, Status=${elevator.status}, DoorStatus=${elevator.doorStatus}, AwaitingDestination=${elevator.isAwaitingDestinationSelection}, CallId=${elevator.currentCallId}, StopType=${elevator.currentStopType}, ExitingCallId=${elevator.exitingCallId}, AwaitingDestCallId=${elevator.awaitingDestinationCallId}`
      );
      console.log(
        `[TabSession ${tabIdRef.current}] Redux state: isInElevator=${reduxStateRef.current.isInElevator}, currentFloor=${reduxStateRef.current.currentFloor}, assignedElevatorId=${currentElevatorIdRef.current}, assignedCallId=${currentCallIdRef.current}, pendingCallId=${pendingCallIdRef.current}, pendingCallFloor=${pendingCallFloorRef.current}`
      );

      setElevators((prevElevators) => {
        const existingIndex = prevElevators.findIndex(
          (e) => e.id === elevator.id
        );
        const prevElevator =
          existingIndex > -1 ? prevElevators[existingIndex] : null;

        if (prevElevator) {
          ElevatorAnalyzer.logElevatorStateTransition(
            elevator,
            prevElevator,
            reduxStateRef.current,
            currentElevatorIdRef,
            pendingCallIdRef,
            tabIdRef.current
          );
        }

        const analysis = ElevatorAnalyzer.analyzeElevatorState(
          elevator,
          prevElevator,
          reduxStateRef.current,
          currentCallIdRef,
          pendingCallIdRef,
          pendingCallFloorRef,
          currentElevatorIdRef
        );

        ElevatorAnalyzer.logDebugInfo(
          elevator,
          analysis,
          pendingCallIdRef,
          currentElevatorIdRef,
          reduxStateRef.current,
          tabIdRef.current
        );

        ElevatorAnalyzer.logStopAnalysis(
          elevator,
          analysis,
          currentCallIdRef,
          pendingCallIdRef,
          currentElevatorIdRef,
          reduxStateRef.current,
          tabIdRef.current
        );

        // Sync call IDs if needed
        if (
          !reduxStateRef.current.isInElevator &&
          !currentElevatorIdRef.current &&
          pendingCallIdRef.current &&
          pendingCallIdRef.current !== currentCallIdRef.current
        ) {
          console.log(
            `[TabSession ${tabIdRef.current}] Syncing callId: updating currentCallId from ${currentCallIdRef.current} to ${pendingCallIdRef.current}`
          );
          currentCallIdRef.current = pendingCallIdRef.current;
          if (pendingCallIdRef.current) {
            sessionStorage.setItem(
              "currentElevatorCallId",
              pendingCallIdRef.current.toString()
            );
          }
        }

        let skipOtherActions = false;

        // Handle different scenarios
        if (analysis.shouldUserStayInElevator) {
          ElevatorActionHandler.handleUserStayInElevator(
            elevator,
            dispatch,
            reduxStateRef,
            tabIdRef.current
          );
          skipOtherActions = true;
        } else if (
          !skipOtherActions &&
          !analysis.shouldPreventDestinationSelection &&
          analysis.isThisUserTheNewPickup
        ) {
          this.logElevatorEntryConsiderations(
            elevator,
            analysis,
            currentCallIdRef,
            pendingCallIdRef,
            reduxStateRef.current,
            tabIdRef.current
          );

          ElevatorActionHandler.handleUserEnterElevator(
            elevator,
            analysis,
            dispatch,
            reduxStateRef,
            currentElevatorIdRef,
            currentCallIdRef,
            pendingCallIdRef,
            pendingCallFloorRef,
            tabIdRef.current
          );
        } else {
          ElevatorActionHandler.logPreventionReasons(
            elevator,
            analysis,
            currentCallIdRef,
            pendingCallIdRef,
            currentElevatorIdRef,
            reduxStateRef.current,
            tabIdRef.current
          );
        }

        // Handle floor updates for users already in elevator
        if (
          !skipOtherActions &&
          reduxStateRef.current.isInElevator &&
          elevator.id === currentElevatorIdRef.current &&
          prevElevator &&
          prevElevator.currentFloor !== elevator.currentFloor &&
          currentCallIdRef.current !== null
        ) {
          ElevatorActionHandler.handleFloorUpdate(
            elevator,
            prevElevator,
            dispatch,
            reduxStateRef,
            currentCallIdRef,
            tabIdRef.current
          );
        }

        // Handle elevator exit
        const shouldExitElevator = this.shouldUserExitElevator(
          elevator,
          prevElevator,
          analysis,
          reduxStateRef.current,
          currentElevatorIdRef,
          currentCallIdRef,
          skipOtherActions
        );

        if (shouldExitElevator) {
          ElevatorActionHandler.handleUserExitElevator(
            elevator,
            dispatch,
            reduxStateRef,
            currentElevatorIdRef,
            currentCallIdRef,
            tabIdRef.current
          );
        } else if (
          this.shouldPreventSameFloorExit(
            elevator,
            prevElevator,
            analysis,
            reduxStateRef.current,
            currentCallIdRef,
            tabIdRef.current,
            skipOtherActions
          )
        ) {
          skipOtherActions = true;
        }

        // Update elevators array
        if (existingIndex > -1) {
          const updatedElevators = [...prevElevators];
          updatedElevators[existingIndex] = elevator;
          return updatedElevators;
        } else {
          return [...prevElevators, elevator];
        }
      });

      // Update logs for significant changes
      this.updateLogsForSignificantChanges(
        elevator,
        currentElevatorIdRef,
        pendingCallIdRef,
        setElevators,
        setLogs
      );
    };
  }

  private static logElevatorEntryConsiderations(
    elevator: Elevator,
    analysis: ElevatorAnalysis,
    currentCallIdRef: ElevatorRef<number | null>,
    pendingCallIdRef: ElevatorRef<number | null>,
    reduxState: ReduxState,
    tabId: string
  ): void {
    console.log(
      `[TabSession ${tabId}] Considering elevator entry: isAwaitingDest=${
        elevator.isAwaitingDestinationSelection
      }, notInElevator=${!reduxState.isInElevator}, noCurrentElevator=${!currentCallIdRef.current}, hasCallId=${!!elevator.currentCallId}, pendingCallMatch=${
        pendingCallIdRef.current === elevator.currentCallId
      }, currentCallMatch=${
        currentCallIdRef.current === elevator.currentCallId
      }, elevatorCallId=${elevator.currentCallId}, myPendingCall=${
        pendingCallIdRef.current
      }, myCurrentCall=${currentCallIdRef.current}, serverBugWorkaround=${
        analysis.serverBugWorkaround
      }, elevatorFloor=${elevator.currentFloor}, myCurrentFloor=${
        reduxState.currentFloor
      }, isPickupStop=${analysis.isPickupStop}, userAtSameFloor=${
        elevator.currentFloor === reduxState.currentFloor
      }`
    );
  }

  private static shouldUserExitElevator(
    elevator: Elevator,
    prevElevator: Elevator | null,
    analysis: ElevatorAnalysis,
    reduxState: ReduxState,
    currentElevatorIdRef: ElevatorRef<number | null>,
    currentCallIdRef: ElevatorRef<number | null>,
    skipOtherActions: boolean
  ): boolean {
    const isExitingRightAfterSameFloorSelection =
      elevator.currentStopType === "destination" &&
      analysis.userStartedOnSameFloor &&
      analysis.justSelectedDestination;

    return (
      !skipOtherActions &&
      reduxState.isInElevator &&
      elevator.id === currentElevatorIdRef.current &&
      currentCallIdRef.current !== null &&
      elevator.status === 3 &&
      elevator.doorStatus === 0 &&
      elevator.exitingCallId === currentCallIdRef.current &&
      prevElevator !== null &&
      (prevElevator.status !== 3 ||
        prevElevator.currentFloor !== elevator.currentFloor) &&
      !isExitingRightAfterSameFloorSelection
    );
  }

  private static shouldPreventSameFloorExit(
    elevator: Elevator,
    prevElevator: Elevator | null,
    analysis: ElevatorAnalysis,
    reduxState: ReduxState,
    currentCallIdRef: ElevatorRef<number | null>,
    tabId: string,
    skipOtherActions: boolean
  ): boolean {
    if (
      !skipOtherActions &&
      reduxState.isInElevator &&
      elevator.id === currentCallIdRef.current &&
      elevator.exitingCallId === currentCallIdRef.current &&
      elevator.currentStopType === "destination" &&
      analysis.userStartedOnSameFloor &&
      (prevElevator?.isAwaitingDestinationSelection === true ||
        analysis.justSelectedDestination)
    ) {
      console.log(
        `[TabSession ${tabId}] PREVENTED SAME FLOOR EXIT: Server wants user to exit at same floor ${elevator.currentFloor} but user just selected a different destination! This is a server bug - user should travel to selected destination first. CallId=${currentCallIdRef.current}, exitingCallId=${elevator.exitingCallId}, stopType=${elevator.currentStopType}, userStartedOnSameFloor=${analysis.userStartedOnSameFloor}, justSelectedDestination=${analysis.justSelectedDestination}`
      );
      return true;
    }
    return false;
  }

  private static updateLogsForSignificantChanges(
    elevator: Elevator,
    currentElevatorIdRef: ElevatorRef<number | null>,
    pendingCallIdRef: ElevatorRef<number | null>,
    setElevators: React.Dispatch<React.SetStateAction<Elevator[]>>,
    setLogs: React.Dispatch<React.SetStateAction<string[]>>
  ): void {
    setElevators((currentElevators) => {
      const existingElevator = currentElevators.find(
        (e: Elevator) => e.id === elevator.id
      );
      const isSignificantChange =
        currentElevatorIdRef.current === elevator.id ||
        pendingCallIdRef.current === elevator.currentCallId ||
        (existingElevator && existingElevator.status !== elevator.status);

      if (isSignificantChange) {
        const elevatorStatus = ElevatorAnalyzer.getElevatorStatusName(
          elevator.status
        );
        setLogs((prev) => [
          `[${new Date().toLocaleTimeString()}] Elevator ${
            elevator.id
          } updated: Floor ${elevator.currentFloor}, Status ${elevatorStatus}${
            elevator.currentCallId ? `, CallId: ${elevator.currentCallId}` : ""
          }`,
          ...prev,
        ]);
      }

      return currentElevators;
    });
  }

  static createConnectedHandler(
    setLogs: React.Dispatch<React.SetStateAction<string[]>>
  ) {
    return (message: string) => {
      setLogs((prev) => [
        `[${new Date().toLocaleTimeString()}] Server: ${message}`,
        ...prev,
      ]);
      toast.success(message);
    };
  }

  static createJoinedGroupHandler(
    setLogs: React.Dispatch<React.SetStateAction<string[]>>
  ) {
    return (groupName: string) => {
      setLogs((prev) => [
        `[${new Date().toLocaleTimeString()}] Joined group: ${groupName}`,
        ...prev,
      ]);
      toast.info(`Joined building group ${groupName.split("_")[1]}`);
    };
  }

  static createErrorHandler(
    setLogs: React.Dispatch<React.SetStateAction<string[]>>
  ) {
    return (errorMessage: string) => {
      setLogs((prev) => [
        `[${new Date().toLocaleTimeString()}] Server Error: ${errorMessage}`,
        ...prev,
      ]);
      toast.error(`Server Error: ${errorMessage}`);
    };
  }

  static createUserArrivedAtDestinationHandler(
    reduxStateRef: ElevatorRef<ReduxState>,
    currentElevatorIdRef: ElevatorRef<number | null>,
    currentCallIdRef: ElevatorRef<number | null>,
    dispatch: Dispatch,
    setLogs: React.Dispatch<React.SetStateAction<string[]>>,
    tabIdRef: ElevatorRef<string>
  ) {
    return (floorNumber: number) => {
      ElevatorActionHandler.handleUserArrivedAtDestination(
        floorNumber,
        dispatch,
        currentElevatorIdRef,
        currentCallIdRef,
        reduxStateRef,
        setLogs,
        tabIdRef.current
      );
    };
  }

  static createElevatorCallAcceptedHandler(
    setLogs: React.Dispatch<React.SetStateAction<string[]>>
  ) {
    return () => {
      setLogs((prev) => [
        `[${new Date().toLocaleTimeString()}] Elevator call accepted - Waiting for elevator arrival`,
        ...prev,
      ]);
    };
  }

  static createElevatorCalledHandler(
    setLogs: React.Dispatch<React.SetStateAction<string[]>>
  ) {
    return () => {
      setLogs((prev) => [
        `[${new Date().toLocaleTimeString()}] Elevator called - Waiting for arrival`,
        ...prev,
      ]);
    };
  }
}
