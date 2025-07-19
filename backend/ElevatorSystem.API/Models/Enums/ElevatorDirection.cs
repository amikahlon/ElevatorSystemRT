namespace ElevatorSystem.API.Models.Enums
{
    public enum ElevatorDirection
    {
        Up,
        Down,
        None
    }

    public enum ElevatorStatus
    {
        Idle,
        MovingUp,
        MovingDown,
        OpeningDoors,
        ClosingDoors
    }

    public enum ElevatorDoorStatus
    {
        Open,
        Closed
    }

    public enum CallDirection
    {
        Up,
        Down,
        None 
    }
}