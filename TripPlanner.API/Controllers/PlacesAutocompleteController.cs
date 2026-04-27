using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripPlanner.Application.Interfaces;

namespace TripPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlacesAutoCompleteController : ControllerBase
    {
        private readonly IPlaceAutoCompleteService _placeAutoCompleteService;
        public PlacesAutoCompleteController(IPlaceAutoCompleteService placeAutoCompleteService)
        {
            _placeAutoCompleteService = placeAutoCompleteService;
        }

        [HttpGet("{query}")]
        public async Task<IActionResult> AutoComplete(string query)
        {
            var result = await _placeAutoCompleteService.AutoCompleteAsync(query);

            return Ok(result);
        }
    }
}
