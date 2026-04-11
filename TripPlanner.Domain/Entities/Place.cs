namespace TripPlanner.Domain.Entities
{
    public class Place
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? ExternalPlaceId { get; set; }
        public string Name { get; set; } = null!;
        public string? Locality { get; set; }
        public string? Country { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
