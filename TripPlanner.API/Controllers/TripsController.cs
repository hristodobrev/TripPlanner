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
        public async Task<IActionResult> Post(TripRequest request, CancellationToken cancellationToken)
        {
            Guid id = await _tripService.AddAsync(request, User.GetUserId(), cancellationToken);

            return Ok(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var trips = await _tripService.GetAllByUserIdAsync(User.GetUserId(), cancellationToken);

            return Ok(trips);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var trip = await _tripService.GetByIdForUserAsync(id, User.GetUserId(), cancellationToken);

            return Ok(trip);
        }

        [HttpGet("shared")]
        public async Task<IActionResult> GetShared(CancellationToken cancellationToken)
        {
            //var trip = await _tripService.GetByIdForUserAsync(id, User.GetUserId(), cancellationToken);

            //return Ok(trip);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove(Guid id, CancellationToken cancellationToken)
        {
            await _tripService.RemoveAsync(id, User.GetUserId(), cancellationToken);

            return Ok();
        }
    }
}
