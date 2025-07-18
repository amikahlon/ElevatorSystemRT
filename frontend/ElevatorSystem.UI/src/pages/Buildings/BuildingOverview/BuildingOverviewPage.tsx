import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import MainLayout from "../../../components/ui/MainLayout";
import * as signalR from "@microsoft/signalr";

const BuildingOverviewPage = () => {
  const { id } = useParams();
  const buildingId = parseInt(id || "0");
  const [log, setLog] = useState<string[]>([]);
  const [, setConnection] = useState<signalR.HubConnection | null>(null);

  useEffect(() => {
    const connect = async () => {
      const hubUrl = `${import.meta.env.VITE_API_URL}/hubs/elevator`;

      const newConnection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl)
        .configureLogging(signalR.LogLevel.Information)
        .build();

      setConnection(newConnection);

      try {
        await newConnection.start();
        setLog((prev) => [...prev, "Connected to Hub!!!!"]);

        await newConnection.invoke("JoinBuildingGroup", buildingId);
        setLog((prev) => [...prev, `Joined group: Building_${buildingId}`]);

        newConnection.on("ReceiveMessage", (message: string) => {
          setLog((prev) => [...prev, `Message from server: ${message}`]);
        });
      } catch (err) {
        setLog((prev) => [...prev, `Connection error: ${err}`]);
      }
    };

    if (buildingId > 0) {
      connect();
    }
  }, [buildingId]);

  return (
    <MainLayout title="ElevatorHub Debug">
      <div className="w-full max-w-3xl bg-white p-10 rounded-xl shadow-xl space-y-4">
        <div className="mt-6 border-t pt-4">
          <h3 className="text-lg font-bold mb-2">Log:</h3>
          <div className="text-sm text-gray-800 space-y-1 max-h-64 overflow-auto">
            {log.map((line, idx) => (
              <p key={idx}>{line}</p>
            ))}
          </div>
        </div>
      </div>
    </MainLayout>
  );
};

export default BuildingOverviewPage;
