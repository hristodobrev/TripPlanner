namespace TripPlanner.Domain.Entities
{
    public class Place
    {
        public Guid Id { get; set; }
        public string? ExternalPlaceId { get; set; }
        public string Name { get; set; } = null!;
        public string? Country { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
