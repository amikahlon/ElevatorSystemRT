namespace ElevatorSystem.API.Models.DTOs.Buildings
{
    public class BuildingDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public int NumberOfFloors { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
