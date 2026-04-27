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
        private readonly IPlaceService _placeService;
        public PlacesController(IPlaceService placeService)
        {
            _placeService = placeService;
        }

        [HttpGet("{externalPlaceId}/{query}")]
        public async Task<IActionResult> Search(string externalPlaceId, string query)
        {
            var result = await _placeService.TextSearchPlacesAsync(externalPlaceId, query);

            return Ok(result);
        }

        [HttpGet("{externalPlaceId}")]
        public async Task<IActionResult> GetPlace(string externalPlaceId)
        {
            var result = await _placeService.GetByExternalIdAsync(externalPlaceId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddPlace(AddPlaceRequest request)
        {
            Guid id = await _placeService.AddAsync(request, User.GetUserId());

            return Ok(id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePlace(Guid id, UpdatePlaceRequest request)
        {
            await _placeService.UpdateAsync(id, request, User.GetUserId());

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemovePlace(Guid id)
        {
            await _placeService.RemoveAsync(id, User.GetUserId());

            return Ok();
        }

        [HttpPut("reorder")]
        public async Task<IActionResult> Reorder(ReorderPlacesRequest request)
        {
            await _placeService.ReorderAsync(request, User.GetUserId());

            var places = await _placeService.GetPlacesForTripAsync(request.TripId!.Value);

            return Ok(places);
        }
    }
}
