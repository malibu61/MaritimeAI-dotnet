using MaritimeAI.BusinessLayer.Abstract;
using MaritimeAI.DtoLayer.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MaritimeAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipsController : ControllerBase
    {
        private readonly IShipsService _shipsService;

        public ShipsController(IShipsService shipsService)
        {
            _shipsService = shipsService;
        }

        [HttpPost("GetShips")]
        public async Task<IActionResult> GetShips(double minLat, double maxLat, double minLon, double maxLon, int zoom)
        {
            var ships = await _shipsService.GetAllShipsAsync(minLat, maxLat, minLon, maxLon, zoom);

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(ships, options);

            return Ok(json);
        }

        [HttpGet("GetAllShipsStream")]
        public async Task GetAllShipsStreamAsync()
        {
            await _shipsService.GetAllShipsStreamAsync(HttpContext);
        }

        [HttpGet("CanakkaleStraitShipsCount")]
        public async Task<IActionResult> CanakkaleStraitShipsCountAsync()
        {
            var southOfCanakkaleStr = await _shipsService.GetShipsCountByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);//South of Canakkale str.
            var northOfCanakkaleStr = await _shipsService.GetShipsCountByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);//North of Canakkale str.
            return Ok(southOfCanakkaleStr + northOfCanakkaleStr);
        }

        [HttpGet("IstanbulStraitShipsCount")]
        public async Task<IActionResult> IstanbulStraitShipsCountAsync()
        {
            var southOfIstanbulStr = await _shipsService.GetShipsCountByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);//South of Istanbul str.
            var middleOfIstanbulStr = await _shipsService.GetShipsCountByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);//Middle of Istanbul str.
            var northOfIstanbulStr = await _shipsService.GetShipsCountByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);//North of Istanbul str.
            return Ok(southOfIstanbulStr + middleOfIstanbulStr + northOfIstanbulStr);
        }

        [HttpGet("CanakkaleStraitShipsAvgSpeed")]
        public async Task<IActionResult> CanakkaleStraitShipsAvgSpeedAsync()
        {
            var southOfCanakkaleStr = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);//South of Canakkale str.
            var northOfCanakkaleStr = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);//North of Canakkale str.
            return Ok((southOfCanakkaleStr + northOfCanakkaleStr) / 2);
        }

        [HttpGet("IstanbulStraitShipsAvgSpeed")]
        public async Task<IActionResult> IstanbulStraitShipsAvgSpeedAsync()
        {
            var southOfIstanbulStr = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);//South of Istanbul str.
            var middleOfIstanbulStr = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);//Middle of Istanbul str.
            var northOfIstanbulStr = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);//North of Istanbul str.
            return Ok((southOfIstanbulStr + middleOfIstanbulStr + northOfIstanbulStr) / 3);
        }

        [HttpGet("CanakkaleStraitTransitShipsCount")]
        public async Task<IActionResult> CanakkaleStraitTransitShipsCountAsync()
        {
            var southOfCanakkaleStr = await _shipsService.GetTransitShipsCountByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);//South of Canakkale str.
            var northOfCanakkaleStr = await _shipsService.GetTransitShipsCountByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);//North of Canakkale str.
            return Ok(southOfCanakkaleStr + northOfCanakkaleStr);
        }

        [HttpGet("IstanbulStraitTransitShipsCount")]
        public async Task<IActionResult> IstanbulStraitTransitShipsCountAsync()
        {
            var southOfIstanbulStr = await _shipsService.GetTransitShipsCountByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);//South of Istanbul str.
            var middleOfIstanbulStr = await _shipsService.GetTransitShipsCountByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);//Middle of Istanbul str.
            var northOfIstanbulStr = await _shipsService.GetTransitShipsCountByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);//North of Istanbul str.
            return Ok(southOfIstanbulStr + middleOfIstanbulStr + northOfIstanbulStr);
        }

        [HttpGet("CanakkaleStraitTankersCount")]
        public async Task<IActionResult> CanakkaleStraitTankersCountAsync()
        {
            var southOfCanakkaleStr = await _shipsService.GetTankersCountByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);//South of Canakkale str.
            var northOfCanakkaleStr = await _shipsService.GetTankersCountByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);//North of Canakkale str.
            return Ok(southOfCanakkaleStr + northOfCanakkaleStr);
        }

        [HttpGet("IstanbulStraitTankersCount")]
        public async Task<IActionResult> IstanbulStraitTankersCountAsync()
        {
            var southOfIstanbulStr = await _shipsService.GetTankersCountByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);//South of Istanbul str.
            var middleOfIstanbulStr = await _shipsService.GetTankersCountByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);//Middle of Istanbul str.
            var northOfIstanbulStr = await _shipsService.GetTankersCountByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);//North of Istanbul str.
            return Ok(southOfIstanbulStr + middleOfIstanbulStr + northOfIstanbulStr);
        }

        [HttpGet("CanakkaleStraitTankersAvgSpeed")]
        public async Task<IActionResult> CanakkaleStraitTankersAvgSpeedAsync()
        {
            var southOfCanakkaleStr = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);//South of Canakkale str.
            var northOfCanakkaleStr = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);//North of Canakkale str.
            return Ok(southOfCanakkaleStr + northOfCanakkaleStr);
        }

        [HttpGet("IstanbulStraitTankersAvgSpeed")]
        public async Task<IActionResult> IstanbulStraitTankersAvgSpeedAsync()
        {
            var southOfIstanbulStr = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);//South of Istanbul str.
            var middleOfIstanbulStr = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);//Middle of Istanbul str.
            var northOfIstanbulStr = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);//North of Istanbul str.
            return Ok(southOfIstanbulStr + middleOfIstanbulStr + northOfIstanbulStr);
        }

    }
}
