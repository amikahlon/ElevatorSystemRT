import { useState, useMemo } from "react";
import type { Elevator as ElevatorType } from "../../types";
import { FaArrowUp, FaArrowDown } from "react-icons/fa";
import { ElevatorCallService } from "../../services/ElevatorCallService";
import { toast } from "react-toastify";
import { useAppSelector } from "../../redux/hooks";

interface Props {
  elevator: ElevatorType;
}

export const Elevator = ({ elevator }: Props) => {
  const isInElevator = useAppSelector((state) => state.auth.isInElevator);
  const numberOfFloors = useAppSelector(
    (state) => state.auth.selectedBuildingFloors
  );

  const [showFloorPicker, setShowFloorPicker] = useState(false);
  const [selectedFloor, setSelectedFloor] = useState<number | null>(null);

  const isIdle = elevator.status === 0;
  const isMoving = elevator.status === 1;
  const isOpeningDoors = elevator.status === 2;
  const isDoorsOpen = elevator.status === 3;
  const isClosingDoors = elevator.status === 4;
  const isOutOfService = elevator.status === 5;

  const isMovingUp = elevator.direction === 1;
  const isMovingDown = elevator.direction === 2;

  const doorsAreOpen = elevator.doorStatus === 0;

  const isMyElevator = isInElevator && elevator.isAwaitingDestinationSelection;

  const floorOptions = useMemo(() => {
    return Array.from({ length: numberOfFloors }, (_, i) => i + 1).filter(
      (floor) => floor !== elevator.currentFloor
    );
  }, [numberOfFloors, elevator.currentFloor]);

  const handleDestinationSelect = async () => {
    if (!selectedFloor || selectedFloor === elevator.currentFloor) {
      toast.error("Please select a valid destination floor");
      return;
    }

    if (!elevator.currentCallId) {
      toast.error("No active call to update");
      console.error("Missing currentCallId for elevator", elevator.id);
      return;
    }

    try {
      console.log(
        `[Elevator Debug] Selecting destination floor ${selectedFloor} for call ${elevator.currentCallId}`
      );
      await ElevatorCallService.selectDestination(
        elevator.currentCallId,
        selectedFloor
      );
      toast.success(`Elevator sent to floor ${selectedFloor}`);
      setShowFloorPicker(false);
      setSelectedFloor(null);
    } catch (error) {
      toast.error("Failed to send destination");
      console.error("Error selecting destination:", error);
    }
  };

  const getStatusText = () => {
    if (isOutOfService) return "Out of Service";
    if (isMoving)
      return isMovingUp ? "Moving Up" : isMovingDown ? "Moving Down" : "Moving";
    if (isOpeningDoors) return "Opening Doors";
    if (isDoorsOpen) return "Doors Open";
    if (isClosingDoors) return "Closing Doors";
    if (isIdle) return "Idle";
    return "Unknown Status";
  };

  const getStatusColor = () => {
    if (isOutOfService) return "text-red-400";
    if (isMoving) return "text-blue-400";
    if (isOpeningDoors || isClosingDoors) return "text-yellow-400";
    if (isDoorsOpen) return "text-green-400";
    if (isIdle) return "text-gray-300";
    return "text-gray-300";
  };

  return (
    <div className="relative w-72 bg-gradient-to-b from-gray-900 to-gray-800 rounded-2xl shadow-2xl border-4 border-gray-700 flex flex-col items-center justify-between p-4">
      <div
        className={`text-sm font-medium tracking-wide mb-2 ${getStatusColor()}`}
      >
        {getStatusText()}
      </div>

      <div className="w-40 h-20 bg-blue-800 rounded-lg flex items-center justify-center text-white text-5xl font-bold border-4 border-blue-500 shadow-inner">
        {elevator.currentFloor}
      </div>

      <div className="flex justify-center items-center gap-4 my-4">
        <FaArrowUp
          className={`text-4xl ${
            isMovingUp
              ? "text-green-400 animate-pulse"
              : "text-gray-500 opacity-40"
          }`}
        />
        <FaArrowDown
          className={`text-4xl ${
            isMovingDown
              ? "text-red-400 animate-pulse"
              : "text-gray-500 opacity-40"
          }`}
        />
      </div>

      <div className="relative w-full h-40 bg-gray-700 rounded-b-xl overflow-hidden border-t-4 border-gray-800">
        <div
          className={`absolute top-0 left-0 w-1/2 h-full bg-gray-600 border-r-2 transition-transform duration-700 ${
            doorsAreOpen ? "-translate-x-full" : "translate-x-0"
          }`}
        ></div>
        <div
          className={`absolute top-0 right-0 w-1/2 h-full bg-gray-600 border-l-2 transition-transform duration-700 ${
            doorsAreOpen ? "translate-x-full" : "translate-x-0"
          }`}
        ></div>
        {(isOpeningDoors || isClosingDoors) && (
          <div className="absolute inset-0 flex items-center justify-center">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-yellow-400"></div>
          </div>
        )}
      </div>

      <div className="mt-2 text-gray-400 text-xs">Elevator #{elevator.id}</div>

      {isMyElevator && (
        <div className="mt-4 w-full text-center">
          {!showFloorPicker ? (
            <button
              className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-full shadow"
              onClick={() => setShowFloorPicker(true)}
            >
              Choose Destination
            </button>
          ) : (
            <div className="flex flex-col items-center space-y-2">
              <select
                value={selectedFloor ?? ""}
                onChange={(e) => setSelectedFloor(Number(e.target.value))}
                className="bg-white text-black rounded px-2 py-1"
              >
                <option value="">Select floor</option>
                {floorOptions.map((floor) => (
                  <option key={floor} value={floor}>
                    {floor}
                  </option>
                ))}
              </select>
              <button
                onClick={handleDestinationSelect}
                className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-1 rounded"
              >
                Go
              </button>
            </div>
          )}
        </div>
      )}
    </div>
  );
};
