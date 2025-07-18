import React from "react";

interface AuthLayoutProps {
  title: string;
  children: React.ReactNode;
}

const AuthLayout: React.FC<AuthLayoutProps> = ({ title, children }) => {
  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 to-gray-700 flex items-center justify-center p-4">
      <div className="bg-white p-10 rounded-xl shadow-2xl w-full max-w-md">
        <h1 className="text-4xl font-extrabold text-center text-blue-700 mb-8 flex items-center justify-center space-x-3">
          <span>Elevator Management System</span>
        </h1>
        <h2 className="text-2xl font-semibold text-center text-gray-700 mb-6">
          {title}
        </h2>
        {children}
      </div>
    </div>
  );
};

export default AuthLayout;
