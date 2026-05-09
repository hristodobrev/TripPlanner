using TripPlanner.Domain.Enums;

namespace TripPlanner.Application.DTOs.Response
{
    public class TripShareResponse
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public string UserFullName { get; set; } = null!;

        public TripPermission Permission { get; set; }
    }
}
