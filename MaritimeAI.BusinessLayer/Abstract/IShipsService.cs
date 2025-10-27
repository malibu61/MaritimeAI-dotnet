using MaritimeAI.DtoLayer.Dtos;
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
        Task<int> GetShipsCountByCoordinatesAsync(double minLat, double maxLat, double minLon, double maxLon,int zoom);
        Task<double> GetShipsAvgSpeedByCoordinatesAsync(double minLat, double maxLat, double minLon, double maxLon,int zoom);
        Task<int> GetTransitShipsCountByCoordinatesAsync(double minLat, double maxLat, double minLon, double maxLon,int zoom);
        Task<int> GetTankersCountByCoordinatesAsync(double minLat, double maxLat, double minLon, double maxLon,int zoom);
        Task<double> GetTankersAvgSpeedByCoordinatesAsync(double minLat, double maxLat, double minLon, double maxLon, int zoom);


    }
}
