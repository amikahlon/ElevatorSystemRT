import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

interface AuthState {
  username: string;
  email: string;
  isLoggedIn: boolean;
  selectedBuildingId: number;
  selectedBuildingName: string;
  currentFloor: number;
}

const initialState: AuthState = {
  username: "",
  email: "",
  isLoggedIn: false,
  selectedBuildingId: 0,
  selectedBuildingName: "",
  currentFloor: 0,
};

const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    login(state, action: PayloadAction<{ username: string; email: string }>) {
      state.username = action.payload.username;
      state.email = action.payload.email;
      state.isLoggedIn = true;
      state.selectedBuildingId = 0;
      state.selectedBuildingName = "";
      state.currentFloor = 0;
    },
    logout(state) {
      state.username = "";
      state.email = "";
      state.isLoggedIn = false;
      state.selectedBuildingId = 0;
      state.selectedBuildingName = "";
      state.currentFloor = 0;
    },
    selectBuilding(state, action: PayloadAction<{ id: number; name: string }>) {
      state.selectedBuildingId = action.payload.id;
      state.selectedBuildingName = action.payload.name;
    },
    setCurrentFloor(state, action: PayloadAction<number>) {
      state.currentFloor = action.payload;
    },
  },
});

export const { login, logout, selectBuilding, setCurrentFloor } =
  authSlice.actions;

export default authSlice.reducer;
