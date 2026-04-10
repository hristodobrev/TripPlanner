using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TripPlanner.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PlacesController : ControllerBase
    {
        [HttpGet]
        public void Search(string query)
        {
            throw new NotImplementedException();
        }
    }
}
