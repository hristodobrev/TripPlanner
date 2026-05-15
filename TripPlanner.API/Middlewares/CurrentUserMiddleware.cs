using System.Security.Claims;
using TripPlanner.Application.Interfaces.Services;

namespace TripPlanner.API.Middlewares
{
    public class CurrentUserMiddleware 
    {
        private readonly RequestDelegate _next;

        public CurrentUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext httpContext,
            ICurrentUserService currentUserService)
        {
            var value = httpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)?
                .Value;

            if (Guid.TryParse(value, out var userId))
            {
                currentUserService.UserId = userId;
            }

            await _next(httpContext);
        }
    }
}
