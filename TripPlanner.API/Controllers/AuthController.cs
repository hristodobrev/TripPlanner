using Microsoft.AspNetCore.Mvc;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Interfaces.Services;

namespace TripPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<AuthResponse> Register(RegisterRequest model, CancellationToken cancellationToken)
        {
            return await _authService.RegisterAsync(model, cancellationToken);
        }

        [HttpPost("login")]
        public async Task<AuthResponse> Login(LoginRequest model, CancellationToken cancellationToken)
        {
            return await _authService.LoginAsync(model, cancellationToken);
        }
    }
}
