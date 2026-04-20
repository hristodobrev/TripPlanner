namespace TripPlanner.Domain.Entities
{
    public class Place
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int? DayNumber { get; set; }
        public int Order { get; set; }

        public string? ExternalId { get; set; }
        public string Name { get; set; } = null!;
        public string? Note { get; set; }

        public Guid TripId { get; set; }
        public Trip Trip { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
