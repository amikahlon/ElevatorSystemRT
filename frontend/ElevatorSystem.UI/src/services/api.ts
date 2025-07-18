import axios from "axios";
import { StorageUtil } from "../utils/storage.util";

let onUnauthorized: (() => void) | null = null;

export const setUnauthorizedHandler = (handler: () => void) => {
  onUnauthorized = handler;
};

const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

api.interceptors.request.use(
  (config) => {
    const token = StorageUtil.getToken();
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      StorageUtil.clearAuthData();

      if (typeof onUnauthorized === "function") {
        onUnauthorized();
      }
    }

    return Promise.reject(error);
  }
);

export default api;
