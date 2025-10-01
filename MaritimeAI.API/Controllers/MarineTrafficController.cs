using MaritimeAI.BusinessLayer.MarineTrafficApiServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MaritimeAI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarineTrafficController : ControllerBase
    {

        private readonly MarineTrafficService _marineTrafficService;

        public MarineTrafficController(MarineTrafficService marineTrafficService)
        {
            _marineTrafficService = marineTrafficService;
        }

        [HttpGet("ships")]
        public async Task<IActionResult> GetShips()
        {
            var ships = await _marineTrafficService.GetShipsAsync();
            return Ok(ships);
        }
    }

}
