using MaritimeAI.BusinessLayer.Abstract;
using MaritimeAI.DtoLAyer.Dtos;
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
            var southOfCanakkaleStr = await _shipsService.GetShipsCountByCoordinatesAsync(39.95, 40.20, 26.12, 26.76, 11);//South of Canakkale str.
            var northOfCanakkaleStr = await _shipsService.GetShipsCountByCoordinatesAsync(40.19, 40.44, 26.35, 26.99, 11);//North of Canakkale str.
            return Ok(southOfCanakkaleStr + northOfCanakkaleStr);
        }

        [HttpGet("IstanbulStraitShipsCount")]
        public async Task<IActionResult> IstanbulStraitShipsCountAsync()
        {
            var southOfIstanbulStr = await _shipsService.GetShipsCountByCoordinatesAsync(41.01, 41.07, 28.93, 29.09, 13);//South of Istanbul str.
            var middleOfIstanbulStr = await _shipsService.GetShipsCountByCoordinatesAsync(41.07, 41.13, 28.99, 29.15, 13);//Middle of Istanbul str.
            var northOfIstanbulStr = await _shipsService.GetShipsCountByCoordinatesAsync(41.13, 41.19, 29.02, 29.18, 13);//North of Istanbul str.
            return Ok(southOfIstanbulStr + middleOfIstanbulStr + northOfIstanbulStr);
        }

        [HttpGet("CanakkaleStraitShipsAvgSpeed")]
        public async Task<IActionResult> CanakkaleStraitShipsAvgSpeedAsync()
        {
            var southOfCanakkaleStr = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(39.95, 40.20, 26.12, 26.76, 11);//South of Canakkale str.
            var northOfCanakkaleStr = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(40.19, 40.44, 26.35, 26.99, 11);//North of Canakkale str.
            return Ok((southOfCanakkaleStr + northOfCanakkaleStr)/2);
        }

        [HttpGet("IstanbulStraitShipsAvgSpeed")]
        public async Task<IActionResult> IstanbulStraitShipsAvgSpeedAsync()
        {
            var southOfIstanbulStr = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(41.01, 41.07, 28.93, 29.09, 13);//South of Istanbul str.
            var middleOfIstanbulStr = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(41.07, 41.13, 28.99, 29.15, 13);//Middle of Istanbul str.
            var northOfIstanbulStr = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(41.13, 41.19, 29.02, 29.18, 13);//North of Istanbul str.
            return Ok((southOfIstanbulStr + middleOfIstanbulStr + northOfIstanbulStr)/3);
        }

        [HttpGet("GetShipsWZoom1Api")]
        public async Task<IActionResult> GetShipsWZoom1Api()
        {
            string url = "https://www.myshiptracking.com/requests/vesselsonmaptempTTT.php?embed=1&type=json&minlat=34.03445260967645&maxlat=42.79540065303723&minlon=22.434082031250004&maxlon=38.16650390625001&zoom=1&selid=null&seltype=null&timecode=-1&slmp=vd8dz&filters=%7B%22vtypes%22%3A%22%2C0%2C3%2C4%2C6%2C7%2C8%2C9%2C10%2C11%2C12%2C13%22%2C%22minsog%22%3A0%2C%22maxsog%22%3A60%2C%22minsz%22%3A10%2C%22maxsz%22%3A500%2C%22minyr%22%3A1950%2C%22maxyr%22%3A2025%2C%22flag%22%3A%22%22%2C%22status%22%3A%22%22%2C%22mapflt_from%22%3A%22%22%2C%22mapflt_dest%22%3A%22%22%7D&_=1759256854012";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

            var response = await client.GetStringAsync(url);

            var ships = new List<ShipsDto>();
            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = Regex.Split(line.Trim(), @"\s+");
                if (parts.Length < 7) continue;

                try
                {
                    double course = parts.Length > 7 ? double.Parse(parts[7].Replace(",", ".")) : 0;

                    while (course > 359)
                    {
                        string courseStr = ((int)course).ToString();
                        if (courseStr.Length > 1)
                            courseStr = courseStr.Substring(0, courseStr.Length - 1);
                        else
                            courseStr = "0";

                        course = double.Parse(courseStr);
                    }

                    var ship = new ShipsDto
                    {
                        Type = int.Parse(parts[0]),
                        Unknown1 = int.Parse(parts[1]),
                        MMSI = long.Parse(parts[2]),
                        Name = parts[3],
                        //Latitude = double.Parse(parts[4].Replace(",", ".")), 
                        //Longitude = double.Parse(parts[5].Replace(",", ".")),
                        Latitude = double.Parse(parts[4].Replace(",", "."), CultureInfo.InvariantCulture),
                        Longitude = double.Parse(parts[5].Replace(",", "."), CultureInfo.InvariantCulture),
                        Speed = double.Parse(parts[6].Replace(",", "."), CultureInfo.InvariantCulture),

                        Course = course
                    };

                    ships.Add(ship);
                }
                catch
                {
                    continue;
                }
            }

            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(ships, options);


            return Ok(json);
        }

    }
}
