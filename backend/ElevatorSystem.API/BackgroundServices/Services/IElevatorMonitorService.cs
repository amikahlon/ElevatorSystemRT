namespace ElevatorSystem.API.BackgroundServices.Services
{
    // Interface for the elevator monitor service
    public interface IElevatorMonitorService
    {

        Task HandleDestinationSelectionAsync(int callId, int destinationFloor);
    }
}
