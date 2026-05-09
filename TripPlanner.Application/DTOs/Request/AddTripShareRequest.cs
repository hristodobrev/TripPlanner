using TripPlanner.Domain.Entities;
using TripPlanner.Domain.Enums;

namespace TripPlanner.Application.DTOs.Request
{
    public class AddTripShareRequest
    {
        public Guid UserId { get; set; }
        public TripPermission Permission { get; set; }
    }
}
