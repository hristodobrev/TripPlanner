using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripPlanner.API.Extensions;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.Interfaces.Services;

namespace TripPlanner.API.Controllers
{
    [Route("api/trips/{tripId:guid}/shares")]
    [ApiController]
    [Authorize]
    public class TripSharesController : ControllerBase
    {
        private readonly ITripShareService _tripShareService;
        public TripSharesController(ITripShareService tripShareService)
        {
            _tripShareService = tripShareService;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid tripId, CancellationToken cancellationToken)
        {
            var result = await _tripShareService.GetByTripIdAsync(tripId, User.GetUserId(), cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Guid tripId, AddTripShareRequest request, CancellationToken cancellationToken)
        {
            await _tripShareService.AddAsync(tripId, request, User.GetUserId(), cancellationToken);

            return Ok();
        }

        [HttpPut("{tripShareId:guid}")]
        public async Task<IActionResult> Update(Guid tripId, Guid tripShareId, UpdateTripShareRequest request, CancellationToken cancellationToken)
        {
            await _tripShareService.UpdateAsync(tripId, tripShareId, request, User.GetUserId(), cancellationToken);

            return Ok();
        }

        [HttpDelete("{tripShareId:guid}")]
        public async Task<IActionResult> Delete(Guid tripId, Guid tripShareId, CancellationToken cancellationToken)
        {
            await _tripShareService.RemoveAsync(tripId, tripShareId, User.GetUserId(), cancellationToken);

            return Ok();
        }
    }
}
