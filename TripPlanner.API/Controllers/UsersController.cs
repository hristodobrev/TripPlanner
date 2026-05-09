using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripPlanner.Application.Interfaces;

namespace TripPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("search/{keyword}")]
        public async Task<IActionResult> Get(string keyword, CancellationToken cancellationToken)
        {
            var result = await _userService.SearchAsync(keyword, cancellationToken);

            return Ok(result);

        }
    }
}
