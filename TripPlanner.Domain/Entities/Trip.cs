namespace TripPlanner.Domain.Entities
{
    public class Trip
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int DurationInDays => (EndDate - StartDate).Days;

        public Guid DestinationPlaceId { get; set; }
        public Place DestinationPlace { get; set; } = null!;
        //public string DestinationExternalId { get; set; } = null!;
        //public string DestinationName { get; set; } = null!;

        public IEnumerable<TripShare> TripShares { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public IEnumerable<TripPlace> Places { get; set; } = null!;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
