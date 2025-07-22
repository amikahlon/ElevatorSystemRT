import type { ReduxState, ElevatorRef } from "../types";
import { SessionStorageManager } from "./sessionStorageManager";

export class StorageEventManager {
  static setupStorageEventListeners(
    currentCallIdRef: ElevatorRef<number | null>,
    pendingCallIdRef: ElevatorRef<number | null>,
    pendingCallFloorRef: ElevatorRef<number | null>,
    reduxStateRef: ElevatorRef<ReduxState>,
    tabIdRef: ElevatorRef<string>
  ): () => void {
    const handleStorageChange = (event: StorageEvent) => {
      if (event.key && SessionStorageManager.isElevatorStorageKey(event.key)) {
        console.log(
          `[TabSession ${tabIdRef.current}] Storage event detected: ${event.key} = ${event.newValue} (previous: ${event.oldValue})`
        );
      }
    };

    const checkForPendingCallChanges = () => {
      SessionStorageManager.updatePendingCallRefs(
        currentCallIdRef,
        pendingCallIdRef,
        pendingCallFloorRef,
        reduxStateRef.current.isInElevator,
        tabIdRef.current
      );
    };

    const interval = setInterval(checkForPendingCallChanges, 100);
    window.addEventListener("storage", handleStorageChange);

    console.log(
      `[TabSession ${tabIdRef.current}] Tab initialized with: pendingCallId=${pendingCallIdRef.current}, pendingCallFloor=${pendingCallFloorRef.current}, currentElevatorId=${currentCallIdRef.current}, currentCallId=${currentCallIdRef.current}`
    );

    // return cleanup function!
    return () => {
      clearInterval(interval);
      window.removeEventListener("storage", handleStorageChange);
    };
  }
}
