using System.ComponentModel.DataAnnotations;
using TripPlanner.Domain.Enums;

namespace TripPlanner.Application.DTOs.Request
{
    public class UpdatePlaceStatusRequest
    {
        [Required]
        public PlaceStatus Status { get; set; }
    }
}
