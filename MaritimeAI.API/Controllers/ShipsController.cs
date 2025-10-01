using MaritimeAI.BusinessLayer.MarineTrafficApiServices;
using MaritimeAI.DtoLAyer.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MaritimeAI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShipsController : ControllerBase
    {
        [HttpGet("GetShips")]
        public async Task<IActionResult> GetShips()
        {
            string url = "https://www.myshiptracking.com/requests/vesselsonmaptempTTT.php?embed=1&type=json&minlat=34.03445260967645&maxlat=42.79540065303723&minlon=22.434082031250004&maxlon=38.16650390625001&zoom=6&selid=null&seltype=null&timecode=-1&slmp=vd8dz&filters=%7B%22vtypes%22%3A%22%2C0%2C3%2C4%2C6%2C7%2C8%2C9%2C10%2C11%2C12%2C13%22%2C%22minsog%22%3A0%2C%22maxsog%22%3A60%2C%22minsz%22%3A10%2C%22maxsz%22%3A500%2C%22minyr%22%3A1950%2C%22maxyr%22%3A2025%2C%22flag%22%3A%22%22%2C%22status%22%3A%22%22%2C%22mapflt_from%22%3A%22%22%2C%22mapflt_dest%22%3A%22%22%7D&_=1759256854012";

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
                        Latitude = double.Parse(parts[4].Replace(",", ".")),
                        Longitude = double.Parse(parts[5].Replace(",", ".")), 
                        Speed = double.Parse(parts[6].Replace(",", ".")), 
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




        [HttpGet("GetShipsWZoom1")]
        public async Task<IActionResult> GetShipsWZoom1()
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
                        Latitude = double.Parse(parts[4].Replace(",", ".")), 
                        Longitude = double.Parse(parts[5].Replace(",", ".")),
                        Speed = double.Parse(parts[6].Replace(",", ".")),
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
