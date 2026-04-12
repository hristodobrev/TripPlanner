using System.Security.Claims;

namespace TripPlanner.API.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
                throw new UnauthorizedAccessException("User ID not found.");

            return Guid.Parse(userId);
        }
    }
}
