using TripPlanner.Domain.Enums;

namespace TripPlanner.Domain.Entities
{
    public class TripPlace
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int? DayNumber { get; set; }
        public int Order { get; set; }

        public string Name { get; set; } = null!;
        public string? Note { get; set; }
        public TimeOnly? PlannedTime { get; set; }
        public int? DurationMinutes { get; set; }
        public PlaceStatus Status { get; set; } = PlaceStatus.Planned;

        public Guid TripId { get; set; }
        public Trip Trip { get; set; } = null!;

        public Guid PlaceId { get; set; }
        public Place Place { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
