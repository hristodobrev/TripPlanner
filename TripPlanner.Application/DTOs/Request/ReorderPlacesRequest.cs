using System.ComponentModel.DataAnnotations;

namespace TripPlanner.Application.DTOs.Request
{
    public class ReorderPlacesRequest
    {
        [Required]
        public Guid? TripId { get; set; }
        public int? DayNumber { get; set; }
        [Required]
        public Guid? SourceId { get; set; }
        public Guid? TargetId { get; set; }
    }
}