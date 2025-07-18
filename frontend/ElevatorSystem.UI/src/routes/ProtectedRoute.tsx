import { Navigate } from "react-router-dom";
import { useAppSelector } from "../redux/hooks";
import type { JSX } from "react";

interface Props {
  children: JSX.Element;
}

const ProtectedRoute = ({ children }: Props) => {
  const isAuthenticated = useAppSelector((state) => state.auth.isLoggedIn);

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return children;
};

export default ProtectedRoute;
