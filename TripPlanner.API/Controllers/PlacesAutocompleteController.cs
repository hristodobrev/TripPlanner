using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripPlanner.Application.Interfaces;

namespace TripPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlacesAutocompleteController : ControllerBase
    {
        private readonly IPlaceAutocompleteService _placeAutocompleteService;
        public PlacesAutocompleteController(IPlaceAutocompleteService placeAutocompleteService)
        {
            _placeAutocompleteService = placeAutocompleteService;
        }

        // TODO: research which is best to return task of IActionResult or List<PlaceAutocompleteResponse>
        [HttpGet("{query}")]
        public async Task<IActionResult> Autocomplete(string query)
        {
            var result = await _placeAutocompleteService.AutocompleteAsync(query);

            return Ok(result);
        }
    }
}
