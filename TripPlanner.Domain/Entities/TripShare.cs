using TripPlanner.Domain.Enums;

namespace TripPlanner.Domain.Entities
{
    public class TripShare
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TripId { get; set; }
        public Trip Trip { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public TripPermission Permission { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
