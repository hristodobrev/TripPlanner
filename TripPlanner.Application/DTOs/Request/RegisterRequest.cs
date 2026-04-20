using System.ComponentModel.DataAnnotations;

namespace TripPlanner.Application.DTOs.Request
{
    public class RegisterRequest
    {
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
    }
}
