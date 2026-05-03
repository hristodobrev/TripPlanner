using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripPlanner.Application.Interfaces;
using TripPlanner.Application.Services;

namespace TripPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlaceSearchController : ControllerBase
    {
        private readonly IPlaceSearchService _placeSearchService;
        public PlaceSearchController(IPlaceSearchService placeSearchService)
        {
            _placeSearchService = placeSearchService;
        }

        [HttpGet("autocomplete/{query}")]
        public async Task<IActionResult> AutoComplete(string query)
        {
            var result = await _placeSearchService.AutoCompleteAsync(query);

            return Ok(result);
        }

        [HttpGet("{latitude}/{longitude}/{query}")]
        public async Task<IActionResult> Search(decimal latitude, decimal longitude, string query)
        {
            // TODO: Add an option to load more places with the page token
            var result = await _placeSearchService.TextSearchPlacesAsync(latitude, longitude, query);

            return Ok(result);
        }

        [HttpGet("recommendations/{latitude}/{longitude}/{destination}")]
        public async Task<IActionResult> SearchRecommendations(decimal latitude, decimal longitude, string destination)
        {
            var result = await _placeSearchService.TextSearchPlacesAsync(latitude, longitude, $"Top attractions in {destination}");

            return Ok(result);
        }

        [HttpGet("accommodations/{latitude}/{longitude}")]
        public async Task<IActionResult> SearchAccommodations(decimal latitude, decimal longitude)
        {
            var result = await _placeSearchService.NearbySearchPlacesAsync(latitude, longitude);

            return Ok(result);
        }

        [HttpGet("{externalPlaceId}")]
        public async Task<IActionResult> GetPlace(string externalPlaceId)
        {
            var result = await _placeSearchService.GetByExternalIdAsync(externalPlaceId);

            return Ok(result);
        }

    }
}
