using Microsoft.AspNetCore.Mvc;
using TripPlanner.Application.Interfaces;

namespace TripPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
    }
}
