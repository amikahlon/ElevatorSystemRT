import { useParams } from "react-router-dom";
import MainLayout from "../../../components/ui/MainLayout";
import { useAppSelector } from "../../../redux/hooks";
import { ElevatorCallService } from "../../../services/ElevatorCallService";
import { toast } from "react-toastify";
import { Elevator } from "../../../components/ui/Elevator";
import { FaArrowUp, FaArrowDown } from "react-icons/fa";
import { useElevatorSignalR } from "../../../hooks/useElevatorSignalR";

const BuildingOverviewPage = () => {
  const { id } = useParams();
  const buildingId = parseInt(id || "0");

  const selectedBuildingId = useAppSelector(
    (state) => state.auth.selectedBuildingId
  );
  const currentFloor = useAppSelector((state) => state.auth.currentFloor);

  const { elevators, connectionStatus, logs } = useElevatorSignalR(buildingId);

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
    <MainLayout title={`Building #${buildingId} - Real-Time Elevator Monitor`}>
      <div className="w-full max-w-5xl mx-auto bg-white p-10 rounded-xl shadow-xl space-y-10">
        {/* Connection Status */}
        <div className="text-center">
          <div className={`font-semibold ${getConnectionStatusColor()}`}>
            Status: {getConnectionStatusText()}
          </div>
          <div className="text-gray-700 font-medium">
            Your Current Floor:{" "}
            <span className="font-bold">{currentFloor}</span>
          </div>
        </div>

        {/* Elevators Display */}
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6 justify-center">
          {elevators.length === 0 ? (
            <div className="col-span-full text-gray-500 text-center py-8">
              {connectionStatus === "connected"
                ? "No elevators available"
                : "Loading elevators..."}
            </div>
          ) : (
            elevators.map((elevator) => (
              <Elevator key={elevator.id} elevator={elevator} />
            ))
          )}
        </div>

        {/* Elevator Call Buttons */}
        <div className="flex flex-col items-center space-y-4">
          <h3 className="text-lg font-semibold">Call Elevator</h3>
          <div className="flex gap-6">
            <button
              onClick={() => handleCallElevator("up")}
              disabled={connectionStatus !== "connected"}
              className={`w-16 h-16 rounded-full flex items-center justify-center text-white shadow-lg transition ${
                connectionStatus === "connected"
                  ? "bg-green-600 hover:bg-green-700 cursor-pointer"
                  : "bg-gray-400 cursor-not-allowed"
              }`}
              title={
                connectionStatus === "connected" ? "Call Up" : "Not connected"
              }
            >
              <FaArrowUp className="text-2xl" />
            </button>
            <button
              onClick={() => handleCallElevator("down")}
              disabled={connectionStatus !== "connected"}
              className={`w-16 h-16 rounded-full flex items-center justify-center text-white shadow-lg transition ${
                connectionStatus === "connected"
                  ? "bg-blue-600 hover:bg-blue-700 cursor-pointer"
                  : "bg-gray-400 cursor-not-allowed"
              }`}
              title={
                connectionStatus === "connected" ? "Call Down" : "Not connected"
              }
            >
              <FaArrowDown className="text-2xl" />
            </button>
          </div>
        </div>

        {/* Activity Log */}
        <div className="border-t pt-6">
          <h3 className="text-lg font-bold mb-2">Activity Log</h3>
          <div className="text-sm text-gray-700 space-y-1 max-h-64 overflow-auto bg-gray-100 p-4 rounded-md">
            {logs.length === 0 ? (
              <p className="text-gray-500">No activity yet...</p>
            ) : (
              logs.map((log, idx) => (
                <p key={idx} className="font-mono text-xs">
                  {log}
                </p>
              ))
            )}
          </div>
        </div>
      </div>
    </MainLayout>
  );
};

export default BuildingOverviewPage;
