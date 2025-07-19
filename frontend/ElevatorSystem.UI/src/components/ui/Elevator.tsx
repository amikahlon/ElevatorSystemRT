import type { Elevator as ElevatorType } from "../../types";
import { FaArrowUp, FaArrowDown } from "react-icons/fa";

interface Props {
  elevator: ElevatorType;
}

export const Elevator = ({ elevator }: Props) => {
  const isMovingUp = elevator.status === 1; // MovingUp
  const isMovingDown = elevator.status === 2; // MovingDown
  const isOpeningDoors = elevator.status === 3; // OpeningDoors
  const isClosingDoors = elevator.status === 4; // ClosingDoors
  const isMaintenance = elevator.status === 5; // Maintenance

  const isDoorsOpen = elevator.doorStatus === 0; // Open

  const getStatusText = () => {
    if (isMovingUp) return "Moving Up";
    if (isMovingDown) return "Moving Down";
    if (isOpeningDoors) return "Opening Doors";
    if (isClosingDoors) return "Closing Doors";
    if (isMaintenance) return "Maintenance";
    return "Idle";
  };

  return (
    <div className="relative w-72 h-96 bg-gradient-to-b from-gray-900 to-gray-800 rounded-2xl shadow-2xl overflow-hidden border-4 border-gray-700 flex flex-col items-center justify-between p-4">
      {/* Status Label */}
      <div className="text-sm text-gray-300 font-medium tracking-wide mb-2">
        {getStatusText()}
      </div>

      {/* Floor Display */}
      <div className="w-40 h-20 bg-blue-800 rounded-lg flex items-center justify-center text-white text-5xl font-bold border-4 border-blue-500 shadow-inner">
        {elevator.currentFloor}
      </div>

      {/* Arrows */}
      <div className="flex justify-center items-center gap-4 my-4">
        <FaArrowUp
          className={`text-4xl transition-all duration-300 ${
            isMovingUp
              ? "text-green-400 animate-pulse drop-shadow-lg"
              : "text-gray-500 opacity-40"
          }`}
        />
        <FaArrowDown
          className={`text-4xl transition-all duration-300 ${
            isMovingDown
              ? "text-red-400 animate-pulse drop-shadow-lg"
              : "text-gray-500 opacity-40"
          }`}
        />
      </div>

      {/* Elevator Doors */}
      <div className="relative w-full h-40 bg-gray-700 rounded-b-xl overflow-hidden border-t-4 border-gray-800">
        <div
          className={`absolute top-0 left-0 w-1/2 h-full bg-gray-600 border-r-2 border-gray-800 transition-transform duration-700 ease-in-out ${
            isDoorsOpen ? "-translate-x-full" : "translate-x-0"
          }`}
        ></div>
        <div
          className={`absolute top-0 right-0 w-1/2 h-full bg-gray-600 border-l-2 border-gray-800 transition-transform duration-700 ease-in-out ${
            isDoorsOpen ? "translate-x-full" : "translate-x-0"
          }`}
        ></div>

        {/* Handles */}
        <div className="absolute top-1/2 left-1/4 -translate-x-1/2 -translate-y-1/2 w-2 h-10 bg-gray-400 rounded-sm z-10"></div>
        <div className="absolute top-1/2 right-1/4 translate-x-1/2 -translate-y-1/2 w-2 h-10 bg-gray-400 rounded-sm z-10"></div>
      </div>

      {/* Info Footer */}
      <div className="absolute bottom-2 text-gray-400 text-xs tracking-wide">
        Elevator #{elevator.id}
      </div>
    </div>
  );
};
