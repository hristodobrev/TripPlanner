
using TripPlanner.Domain.Enums;

namespace TripPlanner.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string? PasswordHash { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? GoogleId { get; set; }
        public AuthProvider AuthProvider { get; set; }
        public bool IsPremium { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
