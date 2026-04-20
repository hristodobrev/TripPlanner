using System.ComponentModel.DataAnnotations;

namespace TripPlanner.Application.DTOs.Request
{
    public class ReorderPlacesRequest
    {
        [Required]
        public Guid? TripId { get; set; }
        [Required]
        public List<DayReorder> Days { get; set; } = null!;
    }

    public class DayReorder
    {
        public int? DayNumber { get; set; }
        [Required]
        public List<Guid> PlaceIds { get; set; } = null!;
    }
}