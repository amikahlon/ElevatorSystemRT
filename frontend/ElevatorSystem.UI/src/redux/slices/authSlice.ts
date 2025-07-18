import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

interface AuthState {
  username: string;
  email: string;
  isLoggedIn: boolean;
  selectedBuildingId: number;
  selectedBuildingName: string;
}

const initialState: AuthState = {
  username: "",
  email: "",
  isLoggedIn: false,
  selectedBuildingId: 0,
  selectedBuildingName: "",
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
    },
    logout(state) {
      state.username = "";
      state.email = "";
      state.isLoggedIn = false;
      state.selectedBuildingId = 0;
      state.selectedBuildingName = "";
    },
    selectBuilding(state, action: PayloadAction<{ id: number; name: string }>) {
      state.selectedBuildingId = action.payload.id;
      state.selectedBuildingName = action.payload.name;
    },
  },
});

export const { login, logout, selectBuilding } = authSlice.actions;
export default authSlice.reducer;
