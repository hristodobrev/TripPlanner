using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TripPlanner.API.Extensions;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.Interfaces;

namespace TripPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;
        public TripsController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpPost]
        public async Task<IActionResult> Post(TripRequest request)
        {
            await _tripService.AddAsync(request, User.GetUserId());

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var trips = await _tripService.GetByUserIdAsync(User.GetUserId());

            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var trip = await _tripService.GetByIdForUserAsync(id, User.GetUserId());

            return Ok(trip);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _tripService.RemoveAsync(id, User.GetUserId());

            return Ok();
        }
    }
}
