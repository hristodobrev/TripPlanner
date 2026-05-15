using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripPlanner.API.Extensions;
using TripPlanner.Application.Interfaces.QueryServices;
using TripPlanner.Application.Interfaces.Services;

namespace TripPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserDashboardQueryService _userDashboardQueryService;
        public UsersController(IUserService userService, IUserDashboardQueryService userDashboardQueryService)
        {
            _userService = userService;
            _userDashboardQueryService = userDashboardQueryService;
        }

        [HttpGet("search/{keyword}")]
        public async Task<IActionResult> Get(string keyword, CancellationToken cancellationToken)
        {
            var result = await _userService.SearchAsync(keyword, cancellationToken);

            return Ok(result);
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardSummary(CancellationToken cancellationToken)
        {
            var result = await _userDashboardQueryService.GetSummaryAsync(User.GetUserId(), cancellationToken);

            return Ok(result);
        }
    }
}
