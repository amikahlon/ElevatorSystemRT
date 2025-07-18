import { useAppSelector, useAppDispatch } from "../../redux/hooks";
import { logout } from "../../redux/slices/authSlice";
import { useNavigate } from "react-router-dom";

const HomePage = () => {
  const user = useAppSelector((state) => state.auth.username);
  const dispatch = useAppDispatch();
  const navigate = useNavigate();

  const handleLogout = () => {
    dispatch(logout());
    navigate("/login");
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 to-gray-700 flex items-center justify-center p-6">
      <div className="bg-white p-10 rounded-xl shadow-2xl w-full max-w-2xl text-center">
        <h1 className="text-4xl font-extrabold text-blue-700 mb-4">
          Welcome to the Elevator System
        </h1>
        <p className="text-xl text-gray-700 mb-8">
          {" "}
          Hello,{" "}
          <span className="font-semibold text-blue-600">{user || "Guest"}</span>
          !
        </p>

        <button
          onClick={handleLogout}
          className="bg-red-600 text-white font-semibold py-2 px-6 rounded-lg hover:bg-red-700 transition duration-300 focus:outline-none focus:ring-2 focus:ring-red-500 focus:ring-offset-2 focus:ring-offset-white"
        >
          Logout
        </button>
      </div>
    </div>
  );
};

export default HomePage;
