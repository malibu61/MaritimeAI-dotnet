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

        [HttpGet("GetShips")]
        public async Task<IActionResult> GetShips()
        {
            string url = "https://www.myshiptracking.com/requests/vesselsonmaptempTTT.php?embed=1&type=json&minlat=34.542762387234845&maxlat=43.24520272203356&minlon=24.906005859375004&maxlon=40.63842773437501&zoom=6&selid=null&seltype=null&timecode=-1&slmp=vd8dz&filters=%7B%22vtypes%22%3A%22%2C0%2C3%2C4%2C6%2C7%2C8%2C9%2C10%2C11%2C12%2C13%22%2C%22minsog%22%3A0%2C%22maxsog%22%3A60%2C%22minsz%22%3A10%2C%22maxsz%22%3A500%2C%22minyr%22%3A1950%2C%22maxyr%22%3A2025%2C%22flag%22%3A%22%22%2C%22status%22%3A%22%22%2C%22mapflt_from%22%3A%22%22%2C%22mapflt_dest%22%3A%22%22%7D&_=1760299521618";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

            var response = await client.GetStringAsync(url);

            var ships = new List<ShipsDto>();
            var lines = response.Split('\n', StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parts = Regex.Split(line.Trim(), @"\s+");
                //if (parts.Length < 7) continue;

                try
                {
                    int type = int.Parse(parts[0]);
                    int unknown1 = int.Parse(parts[1]);
                    long mmsi = long.Parse(parts[2]);

                    int nameStartIndex = 3;
                    int numericStartIndex = nameStartIndex;

                    for (int i = nameStartIndex; i < parts.Length; i++)
                    {
                        if (double.TryParse(parts[i].Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out _))
                        {
                            numericStartIndex = i;
                            break;
                        }
                    }

                    string name = string.Join(" ", parts.Skip(nameStartIndex).Take(numericStartIndex - nameStartIndex));

                    if (parts.Length - numericStartIndex < 4) continue;

                    double latitude = double.Parse(parts[numericStartIndex].Replace(",", "."), CultureInfo.InvariantCulture);
                    double longitude = double.Parse(parts[numericStartIndex + 1].Replace(",", "."), CultureInfo.InvariantCulture);
                    double speed = double.Parse(parts[numericStartIndex + 2].Replace(",", "."), CultureInfo.InvariantCulture);
                    double course = parts.Length > numericStartIndex + 3
                        ? double.Parse(parts[numericStartIndex + 3].Replace(",", "."), CultureInfo.InvariantCulture)
                        : 0;

                    if (double.IsInfinity(latitude) || double.IsNaN(latitude)) latitude = 0;
                    if (double.IsInfinity(longitude) || double.IsNaN(longitude)) longitude = 0;
                    if (double.IsInfinity(speed) || double.IsNaN(speed)) speed = 0;
                    if (double.IsInfinity(course) || double.IsNaN(course)) course = 0;

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
                        Type = type,
                        Unknown1 = unknown1,
                        MMSI = mmsi,
                        Name = name,
                        Latitude = latitude,
                        Longitude = longitude,
                        Speed = speed,
                        Course = course
                    };

                    ships.Add(ship);
                }
                catch
                {
                    continue;
                }
            }

            Console.WriteLine(ships.Count);
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(ships, options);

            return Ok(json);
            //var result = new
            //{
            //    ShipCount = ships.Count,
            //    Ships = ships
            //};

            //return Ok(result);
        }



        [HttpGet("GetShipsWZoom1")]
        public async Task GetShipsWZoom1()
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("Connection", "keep-alive");

            string url = "https://www.myshiptracking.com/requests/vesselsonmaptempTTT.php?embed=1&type=json&minlat=28.998531814051795&maxlat=46.55886030311719&minlon=15.952148437500002&maxlon=47.4169921875&zoom=5&selid=null&seltype=null&timecode=-1&slmp=vd8dz&filters=%7B%22vtypes%22%3A%22%2C0%2C3%2C4%2C6%2C7%2C8%2C9%2C10%2C11%2C12%2C13%22%2C%22minsog%22%3A0%2C%22maxsog%22%3A60%2C%22minsz%22%3A10%2C%22maxsz%22%3A500%2C%22minyr%22%3A1950%2C%22maxyr%22%3A2025%2C%22flag%22%3A%22%22%2C%22status%22%3A%22%22%2C%22mapflt_from%22%3A%22%22%2C%22mapflt_dest%22%3A%22%22%7D&_=1760298482555";

            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
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
                            Latitude = double.Parse(parts[4].Replace(",", "."), CultureInfo.InvariantCulture),
                            Longitude = double.Parse(parts[5].Replace(",", "."), CultureInfo.InvariantCulture),
                            Speed = double.Parse(parts[6].Replace(",", "."), CultureInfo.InvariantCulture),
                            Course = course
                        };

                        ships.Add(ship);
                    }
                    catch { continue; }
                }

                var json = JsonSerializer.Serialize(ships);

                await Response.WriteAsync($"data: {json}\n\n");
                await Response.Body.FlushAsync();

                await Task.Delay(TimeSpan.FromMinutes(1));
            }
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
