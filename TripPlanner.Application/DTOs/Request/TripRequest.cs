using System.ComponentModel.DataAnnotations;

namespace TripPlanner.Application.DTOs.Request
{
    public class TripRequest
    {
        [Required]
        public string Name { get; set; } = null!;
        [MaxLength(1000)]
        public string? Description { get; set; }
        [Required]
        public string PlaceId { get; set; } = null!;
        [Required]
        public string DestinationName { get; set; } = null!;
        [Required]
        public string DestinationCountry { get; set; } = null!;
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
