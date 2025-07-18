import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import {
  HomePage,
  LoginPage,
  RegisterPage,
  AddBuilding,
  BuildingOverviewPage,
} from "../pages";

import ProtectedRoute from "./ProtectedRoute";

const AppRoutes = () => {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />

        <Route
          path="/"
          element={
            <ProtectedRoute>
              <HomePage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/add-building"
          element={
            <ProtectedRoute>
              <AddBuilding />
            </ProtectedRoute>
          }
        />
        <Route
          path="/building/:id"
          element={
            <ProtectedRoute>
              <BuildingOverviewPage />
            </ProtectedRoute>
          }
        />

        <Route path="*" element={<Navigate to="/" />} />
      </Routes>
    </BrowserRouter>
  );
};

export default AppRoutes;
