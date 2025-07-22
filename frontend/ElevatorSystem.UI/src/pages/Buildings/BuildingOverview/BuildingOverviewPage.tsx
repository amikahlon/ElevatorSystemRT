import { useParams } from "react-router-dom";
import MainLayout from "../../../components/ui/MainLayout";
import { useAppSelector, useAppDispatch } from "../../../redux/hooks";
import { ElevatorCallService } from "../../../services/ElevatorCallService";
import { BuildingService } from "../../../services";
import { toast } from "react-toastify";
import { Elevator } from "../../../components/ui/Elevator";
import { FaArrowUp, FaArrowDown, FaEdit, FaPlus } from "react-icons/fa";
import { useElevatorSignalR } from "../../../hooks/elevator";
import { setCurrentFloor } from "../../../redux/slices/authSlice";
import { useState } from "react";

const BuildingOverviewPage = () => {
  const { id } = useParams();
  const buildingId = parseInt(id || "0");

  const selectedBuildingId = useAppSelector(
    (state) => state.auth.selectedBuildingId
  );
  const currentFloor = useAppSelector((state) => state.auth.currentFloor);
  const isInElevator = useAppSelector((state) => state.auth.isInElevator);
  const numberOfFloors = useAppSelector(
    (state) => state.auth.selectedBuildingFloors
  );
  const selectedBuildingName = useAppSelector(
    (state) => state.auth.selectedBuildingName
  );

  const dispatch = useAppDispatch();
  const [showFloorEditor, setShowFloorEditor] = useState(false);
  const [editingFloor, setEditingFloor] = useState<number>(currentFloor);

  const { elevators, connectionStatus } = useElevatorSignalR(buildingId);

  const handleCallElevator = async (direction: "up" | "down") => {
    try {
      await ElevatorCallService.createElevatorCall({
        buildingId: selectedBuildingId,
        requestedFloor: currentFloor,
        direction,
      });
      toast.success(`Elevator called to floor ${currentFloor} (${direction})`);
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (error: any) {
      console.error("Failed to call elevator:", error);
      toast.error("Failed to call elevator");
      console.error("Full response:", error.response?.data);
    }
  };

  const handleAddElevator = async () => {
    try {
      if (selectedBuildingId) {
        await BuildingService.createElevator(selectedBuildingId);
        toast.success("Elevator added successfully! 🚀");
        window.location.reload();
      } else {
        toast.error("No building selected to add an elevator to.");
      }
    } catch (error) {
      console.error("Failed to add elevator:", error);
      toast.error("Failed to add elevator.");
    }
  };

  const handleFloorChange = () => {
    if (editingFloor >= 1 && editingFloor <= numberOfFloors) {
      dispatch(setCurrentFloor(editingFloor));
      setShowFloorEditor(false);
      toast.success(`Moved to floor ${editingFloor} `);
    } else {
      toast.error(`Please enter a valid floor (1-${numberOfFloors})`);
    }
  };

  const getConnectionStatusColor = () => {
    switch (connectionStatus) {
      case "connected":
        return "text-green-600";
      case "connecting":
        return "text-yellow-600";
      case "error":
        return "text-red-600";
      default:
        return "text-gray-600";
    }
  };

  const getConnectionStatusText = () => {
    switch (connectionStatus) {
      case "connected":
        return "Connected";
      case "connecting":
        return "Connecting...";
      case "error":
        return "Connection Error";
      default:
        return "Disconnected";
    }
  };

  return (
    <MainLayout title={selectedBuildingName || `Building #${buildingId}`}>
      <div className="relative w-full max-w-5xl mx-auto bg-white p-8 md:p-10 rounded-xl shadow-xl space-y-8 md:space-y-10">
        <div className="flex flex-col md:flex-row items-center justify-between gap-4 pt-4 md:pt-0">
          <div className="text-center md:text-left flex-grow">
            <div
              className={`font-semibold ${getConnectionStatusColor()} text-lg`}
            >
              Status: {getConnectionStatusText()}
            </div>
            <div className="text-gray-700 font-medium flex items-center justify-center md:justify-start gap-3 mt-2">
              Your Current Floor:{" "}
              {!showFloorEditor ? (
                <div className="flex items-center gap-2">
                  <span className="font-bold text-xl">{currentFloor}</span>
                  {!isInElevator && (
                    <button
                      onClick={() => {
                        setEditingFloor(currentFloor);
                        setShowFloorEditor(true);
                      }}
                      className="text-blue-600 hover:text-blue-800 p-1"
                      title="Edit current floor"
                    >
                      <FaEdit size={16} />
                    </button>
                  )}
                </div>
              ) : (
                <div className="flex items-center gap-2">
                  <input
                    type="number"
                    min="1"
                    max={numberOfFloors}
                    value={editingFloor}
                    onChange={(e) =>
                      setEditingFloor(parseInt(e.target.value) || 1)
                    }
                    className="w-24 px-3 py-1 border border-gray-300 rounded-lg text-center text-base focus:outline-none focus:ring-2 focus:ring-blue-500"
                    autoFocus
                  />
                  <button
                    onClick={handleFloorChange}
                    className="bg-green-600 text-white px-3 py-1 rounded-lg text-sm hover:bg-green-700 transition duration-200"
                  >
                    ✓
                  </button>
                  <button
                    onClick={() => setShowFloorEditor(false)}
                    className="bg-gray-400 text-white px-3 py-1 rounded-lg text-sm hover:bg-gray-500 transition duration-200"
                  >
                    ✕
                  </button>
                </div>
              )}
              {isInElevator && (
                <div className="flex items-center gap-3">
                  <span className="text-orange-600 text-sm font-medium">
                    (In Elevator - Cannot Edit)
                  </span>
                </div>
              )}
            </div>
          </div>

          <button
            onClick={handleAddElevator}
            disabled={!selectedBuildingId || connectionStatus !== "connected"}
            className={`flex items-center gap-2 px-5 py-2 rounded-lg shadow-md text-white font-semibold text-base transition duration-200 ease-in-out
              ${
                selectedBuildingId && connectionStatus === "connected"
                  ? "bg-purple-600 hover:bg-purple-700 cursor-pointer transform hover:scale-105"
                  : "bg-gray-400 cursor-not-allowed"
              }`}
            title={
              connectionStatus === "connected"
                ? "Add a new elevator to this building"
                : "Not connected or no building selected"
            }
          >
            <FaPlus className="text-xl" />
            Add Elevator
          </button>
        </div>

        <hr className="my-6 border-gray-200" />

        <div className="text-center text-2xl font-semibold mb-6 text-gray-800">
          Live Elevators
        </div>
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6 justify-center min-h-[200px] items-start">
          {elevators.length === 0 ? (
            <div className="col-span-full text-gray-500 text-center py-8">
              {connectionStatus === "connected"
                ? "No elevators available in this building yet. Add one!"
                : "Loading elevators or connection not established..."}
            </div>
          ) : (
            elevators.map((elevator) => (
              <Elevator key={elevator.id} elevator={elevator} />
            ))
          )}
        </div>

        <hr className="my-6 border-gray-200" />

        <div className="flex flex-col items-center space-y-6">
          <h3 className="text-2xl font-semibold text-gray-800">
            Call Elevator
          </h3>
          {!isInElevator && (
            <div className="flex flex-wrap justify-center gap-8">
              <button
                onClick={() => handleCallElevator("up")}
                disabled={connectionStatus !== "connected"}
                className={`w-20 h-20 rounded-full flex flex-col items-center justify-center text-white shadow-lg transition duration-200 ease-in-out transform
                  ${
                    connectionStatus === "connected"
                      ? "bg-green-600 hover:bg-green-700 cursor-pointer hover:scale-110"
                      : "bg-gray-400 cursor-not-allowed"
                  }`}
                title={
                  connectionStatus === "connected" ? "Call Up" : "Not connected"
                }
              >
                <FaArrowUp className="text-3xl mb-1" />
                <span className="text-sm">Up</span>
              </button>
              <button
                onClick={() => handleCallElevator("down")}
                disabled={connectionStatus !== "connected"}
                className={`w-20 h-20 rounded-full flex flex-col items-center justify-center text-white shadow-lg transition duration-200 ease-in-out transform
                  ${
                    connectionStatus === "connected"
                      ? "bg-blue-600 hover:bg-blue-700 cursor-pointer hover:scale-110"
                      : "bg-gray-400 cursor-not-allowed"
                  }`}
                title={
                  connectionStatus === "connected"
                    ? "Call Down"
                    : "Not connected"
                }
              >
                <FaArrowDown className="text-3xl mb-1" />
                <span className="text-sm">Down</span>
              </button>
            </div>
          )}
        </div>
      </div>
    </MainLayout>
  );
};

export default BuildingOverviewPage;
