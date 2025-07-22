import { createSlice, type PayloadAction } from "@reduxjs/toolkit";

interface AuthState {
  username: string;
  email: string;
  isLoggedIn: boolean;
  selectedBuildingId: number;
  selectedBuildingName: string;
  selectedBuildingFloors: number;
  currentFloor: number;
  isInElevator: boolean;
}

const initialState: AuthState = {
  username: "",
  email: "",
  isLoggedIn: false,
  selectedBuildingId: 0,
  selectedBuildingName: "",
  selectedBuildingFloors: 10,
  currentFloor: 0,
  isInElevator: false,
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
      state.selectedBuildingFloors = 10;
      state.currentFloor = 2;
      state.isInElevator = false;
    },
    logout(state) {
      state.username = "";
      state.email = "";
      state.isLoggedIn = false;
      state.selectedBuildingId = 0;
      state.selectedBuildingName = "";
      state.selectedBuildingFloors = 10;
      state.currentFloor = 0;
      state.isInElevator = false;
    },
    selectBuilding(
      state,
      action: PayloadAction<{
        id: number;
        name: string;
        numberOfFloors: number;
      }>
    ) {
      state.selectedBuildingId = action.payload.id;
      state.selectedBuildingName = action.payload.name;
      state.selectedBuildingFloors = action.payload.numberOfFloors;
    },
    setCurrentFloor(state, action: PayloadAction<number>) {
      state.currentFloor = action.payload;
    },
    setElevatorStatus(state, action: PayloadAction<boolean>) {
      state.isInElevator = action.payload;
    },
    updateFloorDuringRide(state, action: PayloadAction<number>) {
      if (state.isInElevator) {
        state.currentFloor = action.payload;
      }
    },
    enterElevatorAtCurrentFloor(state) {
      state.isInElevator = true;
    },
  },
});

export const {
  login,
  logout,
  selectBuilding,
  setCurrentFloor,
  setElevatorStatus,
  updateFloorDuringRide,
  enterElevatorAtCurrentFloor,
} = authSlice.actions;

export default authSlice.reducer;
