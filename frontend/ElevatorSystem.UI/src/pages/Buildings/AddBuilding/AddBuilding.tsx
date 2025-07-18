import { useState } from "react";
import { useNavigate } from "react-router-dom";
import MainLayout from "../../../components/ui/MainLayout";
import { BuildingService } from "../../../services";

const AddBuilding = () => {
  const navigate = useNavigate();
  const [name, setName] = useState("");
  const [floors, setFloors] = useState(1);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      await BuildingService.createBuilding({
        name,
        numberOfFloors: floors,
      });
      navigate("/");
    } catch (error) {
      console.error("Failed to create building:", error);
      alert("Something went wrong while creating the building.");
    }
  };

  return (
    <MainLayout title="Add New Building">
      <form
        onSubmit={handleSubmit}
        className="bg-white p-8 rounded-xl shadow-xl w-full max-w-md mx-auto space-y-6"
      >
        <div>
          <label className="block text-gray-700 font-semibold mb-1">
            Building Name
          </label>
          <input
            type="text"
            value={name}
            onChange={(e) => setName(e.target.value)}
            className="w-full border border-gray-300 rounded-lg p-2"
            required
          />
        </div>

        <div>
          <label className="block text-gray-700 font-semibold mb-1">
            Number of Floors
          </label>
          <input
            type="number"
            value={floors}
            onChange={(e) => setFloors(Number(e.target.value))}
            className="w-full border border-gray-300 rounded-lg p-2"
            min={1}
            required
          />
        </div>

        <div className="flex justify-between">
          <button
            type="button"
            onClick={() => navigate("/")}
            className="bg-gray-300 text-gray-700 px-4 py-2 rounded-lg"
          >
            Cancel
          </button>
          <button
            type="submit"
            className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700"
          >
            Add
          </button>
        </div>
      </form>
    </MainLayout>
  );
};

export default AddBuilding;
