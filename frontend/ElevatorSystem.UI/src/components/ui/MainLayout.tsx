import React from "react";

interface MainLayoutProps {
  title: string;
  children: React.ReactNode;
}

const MainLayout: React.FC<MainLayoutProps> = ({ title, children }) => {
  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-900 to-gray-700 p-6 flex flex-col items-center justify-center">
      <div className="bg-white p-10 rounded-xl shadow-2xl w-full max-w-3xl text-center mb-12">
        <h1 className="text-4xl font-extrabold text-blue-700 mb-2">
          Elevator Management System
        </h1>
        <p className="text-lg text-gray-600 mb-2">{title}</p>
      </div>
      {children}
    </div>
  );
};

export default MainLayout;
