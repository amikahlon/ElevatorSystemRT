import type { Elevator } from "../../../types";
import type { ElevatorAnalysis, ReduxState, ElevatorRef } from "../types";

export class ElevatorAnalyzer {
  static analyzeElevatorState(
    elevator: Elevator,
    prevElevator: Elevator | null,
    reduxState: ReduxState,
    currentCallIdRef: ElevatorRef<number | null>,
    pendingCallIdRef: ElevatorRef<number | null>,
    pendingCallFloorRef: ElevatorRef<number | null>,
    currentElevatorIdRef: ElevatorRef<number | null>
  ): ElevatorAnalysis {
    const isAwaitingDestinationNow = elevator.isAwaitingDestinationSelection;
    const wasAwaitingDestinationBefore =
      prevElevator?.isAwaitingDestinationSelection || false;
    const destinationSelectionStateChanged =
      isAwaitingDestinationNow && !wasAwaitingDestinationBefore;
    const isElevatorAtUserFloor =
      elevator.currentFloor === reduxState.currentFloor;
    const isElevatorAtPendingCallFloor =
      pendingCallFloorRef.current !== null &&
      elevator.currentFloor === pendingCallFloorRef.current;
    const isPendingCallMatch =
      pendingCallIdRef.current !== null &&
      elevator.currentCallId === pendingCallIdRef.current;

    const serverBugWorkaround =
      !elevator.isAwaitingDestinationSelection &&
      elevator.status === 3 &&
      elevator.doorStatus === 0 &&
      elevator.currentCallId &&
      isPendingCallMatch &&
      !reduxState.isInElevator;

    const userStartedOnSameFloor =
      elevator.currentFloor === reduxState.currentFloor;
    const justSelectedDestination =
      prevElevator?.isAwaitingDestinationSelection === true &&
      !elevator.isAwaitingDestinationSelection;
    const isPickupStop = elevator.currentStopType === "pickup";
    const shouldUserExit =
      reduxState.isInElevator &&
      elevator.exitingCallId === currentCallIdRef.current;

    const isSameFloorPickup =
      !reduxState.isInElevator &&
      elevator.isAwaitingDestinationSelection &&
      elevator.currentCallId &&
      (pendingCallIdRef.current === elevator.currentCallId ||
        currentCallIdRef.current === elevator.currentCallId) &&
      elevator.currentFloor === reduxState.currentFloor &&
      elevator.status === 3 &&
      elevator.doorStatus === 0 &&
      !currentElevatorIdRef.current;

    const isThisUserTheNewPickup =
      !reduxState.isInElevator &&
      (elevator.isAwaitingDestinationSelection ||
        (elevator.status === 3 &&
          elevator.doorStatus === 0 &&
          elevator.currentCallId &&
          userStartedOnSameFloor)) &&
      elevator.currentCallId &&
      ((elevator.awaitingDestinationCallId &&
        (pendingCallIdRef.current === elevator.awaitingDestinationCallId ||
          currentCallIdRef.current === elevator.awaitingDestinationCallId)) ||
        (!elevator.awaitingDestinationCallId &&
          (pendingCallIdRef.current === elevator.currentCallId ||
            currentCallIdRef.current === elevator.currentCallId))) &&
      (isPickupStop || isSameFloorPickup || userStartedOnSameFloor) &&
      !currentElevatorIdRef.current;

    const isThisUserAlreadyInElevator =
      reduxState.isInElevator &&
      elevator.id === currentElevatorIdRef.current &&
      currentCallIdRef.current !== null;

    const isThisUserAlreadyTravelingToDestination =
      isThisUserAlreadyInElevator && !elevator.isAwaitingDestinationSelection;

    const shouldUserStayInElevator =
      isThisUserAlreadyInElevator &&
      isPickupStop &&
      elevator.exitingCallId !== currentCallIdRef.current;

    const shouldPreventDestinationSelection =
      isThisUserAlreadyTravelingToDestination || shouldUserStayInElevator;

    return {
      isAwaitingDestinationNow,
      wasAwaitingDestinationBefore,
      destinationSelectionStateChanged,
      isElevatorAtUserFloor,
      isElevatorAtPendingCallFloor,
      isPendingCallMatch,
      serverBugWorkaround: Boolean(serverBugWorkaround),
      userStartedOnSameFloor,
      justSelectedDestination,
      isPickupStop,
      shouldUserExit: Boolean(shouldUserExit),
      isSameFloorPickup: Boolean(isSameFloorPickup),
      isThisUserTheNewPickup: Boolean(isThisUserTheNewPickup),
      isThisUserAlreadyInElevator: Boolean(isThisUserAlreadyInElevator),
      isThisUserAlreadyTravelingToDestination: Boolean(
        isThisUserAlreadyTravelingToDestination
      ),
      shouldUserStayInElevator: Boolean(shouldUserStayInElevator),
      shouldPreventDestinationSelection: Boolean(
        shouldPreventDestinationSelection
      ),
    };
  }

  static getElevatorStatusName(status: number): string {
    const elevatorStatusNames = [
      "Idle",
      "Moving",
      "OpeningDoors",
      "DoorsOpen",
      "ClosingDoors",
    ];
    return elevatorStatusNames[status] || status.toString();
  }

  static logElevatorStateTransition(
    elevator: Elevator,
    prevElevator: Elevator,
    reduxState: ReduxState,
    currentElevatorIdRef: ElevatorRef<number | null>,
    pendingCallIdRef: ElevatorRef<number | null>,
    tabId: string
  ): void {
    const isRelatedToThisTab =
      currentElevatorIdRef.current === elevator.id ||
      pendingCallIdRef.current === elevator.currentCallId ||
      elevator.currentFloor === reduxState.currentFloor;

    const elevatorStatus = this.getElevatorStatusName(elevator.status);
    const prevElevatorStatus = this.getElevatorStatusName(prevElevator.status);

    const isUserInThisElevator =
      reduxState.isInElevator &&
      prevElevator &&
      (prevElevator.isAwaitingDestinationSelection ||
        (prevElevator.currentCallId &&
          !prevElevator.isAwaitingDestinationSelection));

    console.log(
      `[TabSession ${tabId}] State transition for elevator ${elevator.id}: ${prevElevatorStatus}->${elevatorStatus}, Floor ${prevElevator.currentFloor}->${elevator.currentFloor}, wasAwaitingDest=${prevElevator.isAwaitingDestinationSelection}, nowAwaitingDest=${elevator.isAwaitingDestinationSelection}, prevCallId=${prevElevator.currentCallId}, currentCallId=${elevator.currentCallId}, isUserInElevator=${isUserInThisElevator}, reduxIsInElevator=${reduxState.isInElevator}, reduxCurrentFloor=${reduxState.currentFloor}, relatedToThisTab=${isRelatedToThisTab}`
    );
  }

  static logDebugInfo(
    elevator: Elevator,
    analysis: ElevatorAnalysis,
    pendingCallIdRef: ElevatorRef<number | null>,
    currentElevatorIdRef: ElevatorRef<number | null>,
    reduxState: ReduxState,
    tabId: string
  ): void {
    if (
      (elevator.isAwaitingDestinationSelection ||
        (elevator.status === 3 &&
          elevator.doorStatus === 0 &&
          elevator.currentCallId)) &&
      !reduxState.isInElevator &&
      elevator.currentCallId
    ) {
      console.log(
        `[TabSession ${tabId}] Elevator ${elevator.id} has doors open with callId but user not entering. Debug: pendingCallId=${pendingCallIdRef.current}, elevatorCallId=${elevator.currentCallId}, isPendingCallMatch=${analysis.isPendingCallMatch}, userFloor=${reduxState.currentFloor}, elevatorFloor=${elevator.currentFloor}, isElevatorAtUserFloor=${analysis.isElevatorAtUserFloor}, isElevatorAtPendingCallFloor=${analysis.isElevatorAtPendingCallFloor}, destinationSelectionStateChanged=${analysis.destinationSelectionStateChanged}, isAwaitingDestination=${elevator.isAwaitingDestinationSelection}, elevatorStatus=${elevator.status}, doorStatus=${elevator.doorStatus}, serverBugWorkaround=${analysis.serverBugWorkaround}, isInElevator=${reduxState.isInElevator}, currentElevatorId=${currentElevatorIdRef.current}`
      );
    }

    if (analysis.serverBugWorkaround) {
      console.log(
        `[TabSession ${tabId}] SERVER BUG WORKAROUND: Elevator ${elevator.id} arrived with our callId ${elevator.currentCallId} but server didn't set AwaitingDestination=true. Forcing entry to elevator. Debug: floor=${elevator.currentFloor}, status=${elevator.status}, doors=${elevator.doorStatus}, awaiting=${elevator.isAwaitingDestinationSelection}`
      );
    }
  }

  static logStopAnalysis(
    elevator: Elevator,
    analysis: ElevatorAnalysis,
    currentCallIdRef: ElevatorRef<number | null>,
    pendingCallIdRef: ElevatorRef<number | null>,
    currentElevatorIdRef: ElevatorRef<number | null>,
    reduxState: ReduxState,
    tabId: string
  ): void {
    console.log(
      `[TabSession ${tabId}] Stop analysis: stopType=${
        elevator.currentStopType
      }, exitingCallId=${elevator.exitingCallId}, isSameFloorPickup=${
        analysis.isSameFloorPickup
      }, shouldUserExit=${analysis.shouldUserExit}, shouldUserStayInElevator=${
        analysis.shouldUserStayInElevator
      }, shouldPreventDestinationSelection=${
        analysis.shouldPreventDestinationSelection
      }, isThisUserTheNewPickup=${
        analysis.isThisUserTheNewPickup
      }, isThisUserAlreadyInElevator=${
        analysis.isThisUserAlreadyInElevator
      }, isThisUserAlreadyTravelingToDestination=${
        analysis.isThisUserAlreadyTravelingToDestination
      }, currentCallId=${currentCallIdRef.current}, pendingCallId=${
        pendingCallIdRef.current
      }, elevatorCallId=${elevator.currentCallId}, isInElevator=${
        reduxState.isInElevator
      }, currentElevatorId=${currentElevatorIdRef.current}, elevatorId=${
        elevator.id
      }, userAtSameFloor=${
        elevator.currentFloor === reduxState.currentFloor
      }, wasAwaitingDestBefore=${
        analysis.wasAwaitingDestinationBefore
      }, nowAwaitingDest=${
        elevator.isAwaitingDestinationSelection
      }, exitingCallIdMatch=${
        elevator.exitingCallId === currentCallIdRef.current
      }, awaitingDestCallId=${
        elevator.awaitingDestinationCallId
      }, awaitingDestCallIdMatch=${
        elevator.awaitingDestinationCallId === pendingCallIdRef.current ||
        elevator.awaitingDestinationCallId === currentCallIdRef.current
      }`
    );
  }
}
