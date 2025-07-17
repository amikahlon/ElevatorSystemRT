namespace ElevatorSystem.API.Models.Entities
{
    public class Building : BaseEntity
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public int NumberOfFloors { get; set; }
    }
}
