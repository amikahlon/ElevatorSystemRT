namespace ElevatorSystem.API.Models.Enums
{
    public enum ElevatorDirection
    {
        None = 0,
        Up = 1,
        Down = 2
    }

    public enum ElevatorStatus
    {
        Idle = 0,
        Moving = 1,
        OpeningDoors = 2,
        DoorsOpen = 3,
        ClosingDoors = 4,
        OutOfService = 5
    }

    public enum CallDirection
    {
        None = 0,
        Up = 1,
        Down = 2
    }
}
    