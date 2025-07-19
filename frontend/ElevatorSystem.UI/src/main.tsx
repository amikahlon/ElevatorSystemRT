import { createRoot } from "react-dom/client";
import "./index.css";

import { Provider } from "react-redux";
import { store, persistor } from "./redux/store";
import { AppRoutes } from "./routes";
import { PersistGate } from "redux-persist/integration/react";

import "react-toastify/dist/ReactToastify.css";
import { ToastContainer } from "react-toastify";

createRoot(document.getElementById("root")!).render(
  <Provider store={store}>
    <PersistGate loading={<div>Loading...</div>} persistor={persistor}>
      <AppRoutes />
    </PersistGate>
    <ToastContainer />
  </Provider>
);
