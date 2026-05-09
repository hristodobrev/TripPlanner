using TripPlanner.Domain.Enums;

namespace TripPlanner.Application.DTOs.Request
{
    public class UpdateTripShareRequest
    {
        public TripPermission Permission { get; set; }
    }
}
