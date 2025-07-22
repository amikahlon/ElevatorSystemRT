# Elevator System Real-Time

## Project Description

This is a real-time elevator system with a web interface. It helps you keep an eye on and manage elevators in buildings, letting you call them, see where they are, and even watch them move around.

**Interactive Experience:** Want to try it out? Open the app in two different browser tabs! You can then call elevators from different floors in each tab and see how everything updates instantly.

## Installation & Setup

### Backend Setup

#### Navigate to backend

```bash
cd backend/ElevatorSystem.API
```

#### Create .env file

Create `.env` file in `backend/ElevatorSystem.API`

#### Restore packages

```bash
dotnet restore
```

#### Create database

```bash
dotnet ef database update
```

#### Run server

```bash
dotnet run
```

Server available at: `http://localhost:5066`
API Documentation: `http://localhost:5066/swagger`

### Frontend Setup

#### Navigate to frontend

```bash
cd ../../frontend/ElevatorSystem.UI
```

#### Install dependencies

```bash
npm i
```

#### Create .env file

Create `.env` file in `frontend/ElevatorSystem.UI`

#### Run dev server

```bash
npm run dev
```

Application available at: `http://localhost:5173`

---

Developed by Ami Kahlon
