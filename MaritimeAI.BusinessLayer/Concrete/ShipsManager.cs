using Azure;
using MaritimeAI.BusinessLayer.Abstract;
using MaritimeAI.DtoLayer.Dtos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;


namespace MaritimeAI.BusinessLayer.Concrete
{
    public class ShipsManager : IShipsService
    {

        private readonly HttpClient client;

        public ShipsManager(HttpClient httpClient)
        {
            client = httpClient;
        }

        public async Task<List<ShipsDto>> GetAllShipsAsync(double minLat = 40, double maxLat = 45, double minLon = 20, double maxLon = 25, int zoom = 5)
        {
            try
            {
                long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                string url = $"https://ships-tracking-api.p.rapidapi.com/vessels?minLat={minLat}&maxLat={maxLat}&minLon={minLon}&maxLon={maxLon}&zoom={zoom}&_={timestamp}";

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", "your-api-key-here");
                client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "ships-tracking-api.p.rapidapi.com");

                var response = await client.GetStringAsync(url);

                var ships = JsonSerializer.Deserialize<List<ShipsDto>>(response, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return ships ?? new List<ShipsDto>();
            }
            catch (Exception ex)
            {
                return new List<ShipsDto>();
            }
        }



        public async Task<int> GetShipsCountByCoordinatesAsync(double minLat, double maxLat, double minLon, double maxLon, int zoom)
        {
            var allShips = await GetAllShipsAsync(minLat, maxLat, minLon, maxLon, zoom);
            var regionShips = allShips.Where(x => true).Count();

            return regionShips;
        }

        public async Task<double> GetShipsAvgSpeedByCoordinatesAsync(double minLat, double maxLat, double minLon, double maxLon, int zoom)
        {
            var allShips = await GetAllShipsAsync(minLat, maxLat, minLon, maxLon, zoom);
            var filteredShips = allShips.Where(x => x.Type == 7 || x.Type == 8).ToList();
            var avgSpeed = filteredShips.Any() ? filteredShips.Average(x => x.Speed) : 10;
            return avgSpeed;
        }

        public async Task<int> GetTransitShipsCountByCoordinatesAsync(double minLat, double maxLat, double minLon, double maxLon, int zoom)
        {
            var allShips = await GetAllShipsAsync(minLat, maxLat, minLon, maxLon, zoom);
            var regionShips = allShips.Where(x => x.Type == 7 || x.Type == 8).Count();

            return regionShips;
        }

        public async Task<int> GetTankersCountByCoordinatesAsync(double minLat, double maxLat, double minLon, double maxLon, int zoom)
        {
            var allShips = await GetAllShipsAsync(minLat, maxLat, minLon, maxLon, zoom);
            var regionShips = allShips.Where(x => x.Type == 8).Count();

            return regionShips;
        }

        public async Task<double> GetTankersAvgSpeedByCoordinatesAsync(double minLat, double maxLat, double minLon, double maxLon, int zoom)
        {
            var allShips = await GetAllShipsAsync(minLat, maxLat, minLon, maxLon, zoom);
            var filteredShips = allShips.Where(x => x.Type == 8).ToList();
            var avgSpeed = filteredShips.Any() ? filteredShips.Average(x => x.Speed) : 10;
            return avgSpeed;
        }
    }
}
