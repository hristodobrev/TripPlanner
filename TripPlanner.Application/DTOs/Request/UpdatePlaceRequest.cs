using System.ComponentModel.DataAnnotations;

namespace TripPlanner.Application.DTOs.Request
{
    public class UpdatePlaceRequest
    {
        [Required]
        public Guid? PlaceId { get; set; }
        public string? Note { get; set; }
    }
}
