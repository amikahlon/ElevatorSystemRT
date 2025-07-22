import type { ElevatorState, ElevatorRef } from "../types";

export class SessionStorageManager {
  private static readonly KEYS = {
    TAB_SESSION_ID: "tabSessionId",
    CURRENT_ELEVATOR_CALL_ID: "currentElevatorCallId",
    CURRENT_ELEVATOR_ID: "currentElevatorId",
    PENDING_CALL_ID: "pendingCallId",
    PENDING_CALL_FLOOR: "pendingCallFloor",
    PENDING_ELEVATOR_CALL_ID: "pendingElevatorCallId",
    PENDING_ELEVATOR_CALL_FLOOR: "pendingElevatorCallFloor",
    PENDING_CALL_DATA: "pendingCallData",
    PENDING_ELEVATOR_CALL_DATA: "pendingElevatorCallData",
  } as const;

  static initializeTabId(): string {
    const existingId = sessionStorage.getItem(this.KEYS.TAB_SESSION_ID);
    if (existingId) return existingId;

    const newId = Math.random().toString(36).substr(2, 9);
    sessionStorage.setItem(this.KEYS.TAB_SESSION_ID, newId);
    return newId;
  }

  static initializeElevatorState(): ElevatorState {
    const currentCallId = sessionStorage.getItem(
      this.KEYS.CURRENT_ELEVATOR_CALL_ID
    );
    const currentElevatorId = sessionStorage.getItem(
      this.KEYS.CURRENT_ELEVATOR_ID
    );
    const pendingCallId =
      sessionStorage.getItem(this.KEYS.PENDING_CALL_ID) ||
      sessionStorage.getItem(this.KEYS.PENDING_ELEVATOR_CALL_ID);
    const pendingCallFloor =
      sessionStorage.getItem(this.KEYS.PENDING_CALL_FLOOR) ||
      sessionStorage.getItem(this.KEYS.PENDING_ELEVATOR_CALL_FLOOR);

    return {
      currentCallId: currentCallId ? parseInt(currentCallId) : null,
      currentElevatorId: currentElevatorId ? parseInt(currentElevatorId) : null,
      pendingCallId: pendingCallId ? parseInt(pendingCallId) : null,
      pendingCallFloor: pendingCallFloor ? parseInt(pendingCallFloor) : null,
      tabId: this.initializeTabId(),
    };
  }

  static updateCurrentElevatorCall(
    elevatorId: number,
    callId: number | null
  ): void {
    sessionStorage.setItem(
      this.KEYS.CURRENT_ELEVATOR_ID,
      elevatorId.toString()
    );
    if (callId) {
      sessionStorage.setItem(
        this.KEYS.CURRENT_ELEVATOR_CALL_ID,
        callId.toString()
      );
    }
  }

  static clearElevatorState(): void {
    sessionStorage.removeItem(this.KEYS.CURRENT_ELEVATOR_ID);
    sessionStorage.removeItem(this.KEYS.CURRENT_ELEVATOR_CALL_ID);
  }

  static clearPendingCall(): void {
    sessionStorage.removeItem(this.KEYS.PENDING_CALL_ID);
    sessionStorage.removeItem(this.KEYS.PENDING_CALL_FLOOR);
  }

  static clearAllElevatorData(): void {
    this.clearElevatorState();
    this.clearPendingCall();
    sessionStorage.removeItem(this.KEYS.PENDING_ELEVATOR_CALL_ID);
    sessionStorage.removeItem(this.KEYS.PENDING_ELEVATOR_CALL_FLOOR);
  }

  static isElevatorStorageKey(key: string): boolean {
    return Object.values(this.KEYS).includes(
      key as (typeof this.KEYS)[keyof typeof this.KEYS]
    );
  }

  static getCurrentPendingCallData(): {
    id: string | null;
    floor: string | null;
  } {
    return {
      id:
        sessionStorage.getItem(this.KEYS.PENDING_CALL_ID) ||
        sessionStorage.getItem(this.KEYS.PENDING_ELEVATOR_CALL_ID),
      floor:
        sessionStorage.getItem(this.KEYS.PENDING_CALL_FLOOR) ||
        sessionStorage.getItem(this.KEYS.PENDING_ELEVATOR_CALL_FLOOR),
    };
  }

  static updatePendingCallRefs(
    currentCallIdRef: ElevatorRef<number | null>,
    pendingCallIdRef: ElevatorRef<number | null>,
    pendingCallFloorRef: ElevatorRef<number | null>,
    isInElevator: boolean,
    tabId: string
  ): void {
    if (isInElevator) return;

    const pendingData = this.getCurrentPendingCallData();

    if (
      pendingData.id &&
      pendingCallIdRef.current !== parseInt(pendingData.id)
    ) {
      console.log(
        `[TabSession ${tabId}] Updating pendingCallId from ${pendingCallIdRef.current} to ${pendingData.id} (not in elevator)`
      );
      pendingCallIdRef.current = parseInt(pendingData.id);
    }

    if (
      pendingData.floor &&
      pendingCallFloorRef.current !== parseInt(pendingData.floor)
    ) {
      console.log(
        `[TabSession ${tabId}] Updating pendingCallFloor from ${pendingCallFloorRef.current} to ${pendingData.floor} (not in elevator)`
      );
      pendingCallFloorRef.current = parseInt(pendingData.floor);
    }

    if (
      !currentCallIdRef.current &&
      pendingCallIdRef.current &&
      pendingCallIdRef.current !== currentCallIdRef.current
    ) {
      console.log(
        `[TabSession ${tabId}] Syncing callId: updating currentCallId from ${currentCallIdRef.current} to ${pendingCallIdRef.current}`
      );
      currentCallIdRef.current = pendingCallIdRef.current;
      if (pendingCallIdRef.current) {
        sessionStorage.setItem(
          this.KEYS.CURRENT_ELEVATOR_CALL_ID,
          pendingCallIdRef.current.toString()
        );
      }
    }
  }
}
