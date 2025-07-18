import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

interface AuthState {
  username: string;
  email: string;
  isLoggedIn: boolean;
  selectedBuildingId: number;
}

const initialState: AuthState = {
  username: "",
  email: "",
  isLoggedIn: false,
  selectedBuildingId: 0,
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
    },
    logout(state) {
      state.username = "";
      state.email = "";
      state.isLoggedIn = false;
      state.selectedBuildingId = 0;
    },
    selectBuilding(state, action: PayloadAction<number>) {
      state.selectedBuildingId = action.payload;
    },
  },
});

export const { login, logout, selectBuilding } = authSlice.actions;
export default authSlice.reducer;
