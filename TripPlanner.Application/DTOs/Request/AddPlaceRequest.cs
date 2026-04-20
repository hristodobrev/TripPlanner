using System.ComponentModel.DataAnnotations;

namespace TripPlanner.Application.DTOs.Request
{
    public class AddPlaceRequest
    {
        [Required]
        public Guid? TripId { get; set; }
        [Required]
        public string ExternalId { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
    }
}
