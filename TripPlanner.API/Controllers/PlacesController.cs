using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripPlanner.API.Extensions;
using TripPlanner.Application.DTOs.Request;
using TripPlanner.Application.Interfaces;

namespace TripPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlacesController : ControllerBase
    {
        private readonly ITripPlaceService _placeService;
        public PlacesController(ITripPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPlace(AddPlaceRequest request, CancellationToken cancellationToken)
        {
            Guid id = await _placeService.AddAsync(request, User.GetUserId(), cancellationToken);

            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlace(Guid id, UpdatePlaceRequest request, CancellationToken cancellationToken)
        {
            await _placeService.UpdateAsync(id, request, User.GetUserId(), cancellationToken);

            return Ok();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdatePlaceStatus(Guid id, UpdatePlaceStatusRequest request, CancellationToken cancellationToken)
        {
            await _placeService.UpdateStatusAsync(id, request, User.GetUserId(), cancellationToken);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemovePlace(Guid id, CancellationToken cancellationToken)
        {
            await _placeService.RemoveAsync(id, User.GetUserId(), cancellationToken);

            return Ok();
        }

        [HttpPut("reorder")]
        public async Task<IActionResult> Reorder(ReorderPlacesRequest request, CancellationToken cancellationToken)
        {
            await _placeService.ReorderAsync(request, User.GetUserId(), cancellationToken);

            var places = await _placeService.GetPlacesForTripAsync(request.TripId!.Value, cancellationToken);
            return Ok(places);
        }
    }
}
