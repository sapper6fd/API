using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Outmatch.API.Data;

namespace Outmatch.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationsRepository _repo;
        public LocationsController(ILocationsRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetLocations()
        {
            var locations = await _repo.GetLocations();
            return Ok(locations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetLocation(int id)
        {
            var location = await _repo.GetLocation(id);
            return Ok(location);
        }
    }
}