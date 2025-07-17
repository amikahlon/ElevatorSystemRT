namespace ElevatorSystem.API.Configuration
{
    public class DatabaseSettings
    {
        public const string SectionName = "Database";

        public string ConnectionString { get; set; } = string.Empty;
        public int CommandTimeout { get; set; } = 30;
        public bool EnableSensitiveDataLogging { get; set; } = false;
        public bool EnableDetailedErrors { get; set; } = false;
    }
}