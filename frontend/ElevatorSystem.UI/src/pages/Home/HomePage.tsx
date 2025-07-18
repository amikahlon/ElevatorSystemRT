import { useEffect, useState } from "react";
import { useAppSelector, useAppDispatch } from "../../redux/hooks";
import { logout, selectBuilding } from "../../redux/slices/authSlice";
import { useNavigate } from "react-router-dom";
import { BuildingService } from "../../services";
import type { Building } from "../../types";
import { Building2, Plus } from "lucide-react";
import MainLayout from "../../components/ui/MainLayout";

const HomePage = () => {
  const user = useAppSelector((state) => state.auth.username);
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const [buildings, setBuildings] = useState<Building[]>([]);

  useEffect(() => {
    BuildingService.getMyBuildings().then(setBuildings).catch(console.error);
  }, []);

  const handleLogout = () => {
    dispatch(logout());
    navigate("/login");
  };

  const handleSelect = (buildingId: number, buildingName: string) => {
    dispatch(selectBuilding({ id: buildingId, name: buildingName }));
    navigate(`/building/${buildingId}`);
  };

  const handleAddBuilding = () => {
    navigate("/add-building");
  };

  return (
    <MainLayout title={`Hello, ${user || "Guest"}`}>
      <div className="bg-white p-10 rounded-xl shadow-xl w-full max-w-5xl mb-10">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-2xl font-bold text-gray-800">
            Choose a Building
          </h2>
          <button
            onClick={handleAddBuilding}
            className="flex items-center space-x-2 bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 transition"
          >
            <Plus size={18} />
            <span>Add Building</span>
          </button>
        </div>

        {buildings.length === 0 ? (
          <p className="text-gray-500 text-center">No buildings available.</p>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6">
            {buildings.map((b) => (
              <div
                key={b.id}
                onClick={() => handleSelect(b.id, b.name)}
                className="cursor-pointer p-6 bg-gradient-to-br from-blue-50 to-blue-100 border border-blue-200 rounded-2xl shadow-md hover:shadow-lg transition hover:scale-105"
              >
                <div className="flex flex-col items-center justify-center space-y-4">
                  <Building2 size={48} className="text-blue-600" />
                  <h3 className="text-xl font-semibold text-gray-800">
                    {b.name}
                  </h3>
                  <p className="text-sm text-gray-600">
                    Floors: {b.numberOfFloors}
                  </p>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      <div className="w-full flex justify-center">
        <button
          onClick={handleLogout}
          className="bg-red-600 text-white font-medium py-2 px-6 rounded-lg hover:bg-red-700 transition"
        >
          Logout
        </button>
      </div>
    </MainLayout>
  );
};

export default HomePage;
