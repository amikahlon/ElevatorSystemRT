import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { AuthService } from "../../services";
import { useAppDispatch } from "../../redux/hooks";
import { login } from "../../redux/slices/authSlice";
import type { HttpErrorResponse } from "../../types";
import AuthLayout from "../../components/ui/AuthLayout";

const LoginPage = () => {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();
  const dispatch = useAppDispatch();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    try {
      const response = await AuthService.login({ email, password });

      dispatch(
        login({ username: response.user.name, email: response.user.email })
      );

      navigate("/");
    } catch (err) {
      const error = err as HttpErrorResponse;

      if (error.response?.data?.message) {
        setError(error.response.data.message);
      } else if (error.message) {
        setError(error.message);
      } else {
        setError("Login failed. Please try again.");
      }
    }
  };

  return (
    <AuthLayout title="Login to your account">
      <form onSubmit={handleSubmit} className="space-y-6">
        <div>
          <label
            htmlFor="email"
            className="block text-gray-700 text-sm font-medium mb-2"
          >
            Email:
          </label>
          <input
            id="email"
            type="email"
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            required
            placeholder="Enter your email"
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-800"
          />
        </div>

        <div>
          <label
            htmlFor="password"
            className="block text-gray-700 text-sm font-medium mb-2"
          >
            Password:
          </label>
          <input
            id="password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
            placeholder="Enter your password"
            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-gray-800"
          />
        </div>

        {error && (
          <p className="text-red-600 text-center text-sm mt-4">{error}</p>
        )}

        <button
          type="submit"
          className="w-full bg-blue-600 text-white font-semibold py-3 px-4 rounded-lg hover:bg-blue-700 transition duration-300 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 focus:ring-offset-white"
        >
          Login
        </button>
      </form>

      <p className="text-center text-gray-600 mt-8 text-sm">
        Don't have an account?{" "}
        <Link
          to="/register"
          className="text-blue-600 hover:underline font-medium"
        >
          Register here
        </Link>
      </p>
    </AuthLayout>
  );
};

export default LoginPage;
