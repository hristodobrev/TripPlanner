using System.ComponentModel.DataAnnotations;

namespace TripPlanner.Application.DTOs.Request
{
    public class TripRequest
    {
        public string Name { get; set; } = null!;
        [MaxLength(1000)]
        public string? Description { get; set; }
        public string PlaceId { get; set; } = null!;
        public string PlaceName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
