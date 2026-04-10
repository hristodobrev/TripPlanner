using Microsoft.AspNetCore.Mvc;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.DTOs.Response;
using TripPlanner.Application.Interfaces;

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
        public async Task<AuthResponse> Register(RegisterRequest model)
        {
            // TODO: Add validation and error handling

            return await _authService.RegisterAsync(model);
        }

        [HttpPost("login")]
        public async Task<AuthResponse> Login(LoginRequest model)
        {
            // TODO: Add validation and error handling

            return await _authService.LoginAsync(model);
        }

        [HttpPost("google")]
        public AuthResponse Google(GoogleLoginRequest model)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        public AuthResponse Me()
        {
            throw new NotImplementedException();
        }
    }
}
