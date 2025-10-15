using MaritimeAI.DtoLAyer.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaritimeAI.BusinessLayer.Abstract
{
    public interface IShipsService
    {
        Task<List<ShipsDto>> GetAllShipsAsync(double minLat, double maxLat, double minLon, double maxLon, int zoom);
        Task GetAllShipsStreamAsync(HttpContext httpContext);

        Task<int> GetShipsCountByCoordinatesAsync(double minLat, double maxLat, double minLon, double maxLon,int zoom);
        Task<double> GetShipsAvgSpeedByCoordinatesAsync(double minLat, double maxLat, double minLon, double maxLon,int zoom);
        
    }
}
