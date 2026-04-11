namespace TripPlanner.Domain.Entities
{
    public class Trip
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int DurationInDays => (EndDate - StartDate).Days;

        public Guid PlaceId { get; set; }
        public Place Place { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
