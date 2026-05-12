namespace TripPlanner.Domain.Entities
{
    public class PlaceDetails
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string ExternalId { get; set; } = null!;
        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
