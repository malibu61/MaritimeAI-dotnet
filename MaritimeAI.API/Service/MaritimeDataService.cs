using MaritimeAI.API.Hubs;
using MaritimeAI.BusinessLayer.Abstract;
using Microsoft.AspNetCore.SignalR;

namespace MaritimeAI.API.Service
{
    public class MaritimeDataService : BackgroundService
    {
        private readonly IHubContext<MaritimeHub> _hubContext;
        private readonly IShipsService _shipsService;

        public MaritimeDataService(IHubContext<MaritimeHub> hubContext, IShipsService shipsService)
        {
            _hubContext = hubContext;
            _shipsService = shipsService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                //Çanakkale
                var southOfCanakkaleStrAllShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(39.95, 40.20, 26.12, 26.76, 11);//South of Canakkale str.
                var northOfCanakkaleStrAllShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(40.19, 40.44, 26.35, 26.99, 11);//North of Canakkale str.


                var southOfCanakkaleStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(39.54, 40.9, 26.10, 26.49, 11);//South of Canakkale str.
                var northOfCanakkaleStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(40.9, 40.24, 26.19, 26.57, 11);//North of Canakkale str.


                var southOfCanakkaleStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(39.54, 40.9, 26.10, 26.49, 11);//South of Canakkale str.
                var northOfCanakkaleStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(40.9, 40.24, 26.19, 26.57, 11);//North of Canakkale str.


                var southOfCanakkaleStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(39.95, 40.20, 26.12, 26.76, 11);//South of Canakkale str.
                var northOfCanakkaleStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(40.19, 40.44, 26.35, 26.99, 11);//North of Canakkale str.


                var southOfCanakkaleStrTankersAvgSpeed = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(39.54, 40.9, 26.10, 26.49, 11);//South of Canakkale str.
                var northOfCanakkaleStrTankersAvgSpeed = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(40.9, 40.24, 26.19, 26.57, 11);//North of Canakkale str.



                //İstanbul

                var southOfIstanbulStrAllShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(41.01, 41.07, 28.93, 29.09, 13);//South of Istanbul str.
                var middleOfIstanbulStrAllShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(41.07, 41.13, 28.99, 29.15, 13);//Middle of Istanbul str.
                var northOfIstanbulStrAllShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(41.13, 41.19, 29.02, 29.18, 13);//North of Istanbul str.


                var southOfIstanbulStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(41.01, 41.07, 28.93, 29.09, 13);//South of Istanbul str.
                var middleOfIstanbulStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(41.07, 41.13, 28.99, 29.15, 13);//Middle of Istanbul str.
                var northOfIstanbulStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(41.13, 41.19, 29.02, 29.18, 13);//North of Istanbul str.


                var southOfIstanbulStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(41.01, 41.07, 28.93, 29.09, 13);//South of Istanbul str.
                var middleOfIstanbulStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(41.07, 41.13, 28.99, 29.15, 13);//Middle of Istanbul str.
                var northOfIstanbulStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(41.13, 41.19, 29.02, 29.18, 13);//North of Istanbul str.


                var southOfIstanbulStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(41.01, 41.07, 28.93, 29.09, 13);//South of Istanbul str.
                var middleOfIstanbulStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(41.07, 41.13, 28.99, 29.15, 13);//Middle of Istanbul str.
                var northOfIstanbulStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(41.13, 41.19, 29.02, 29.18, 13);//North of Istanbul str.


                var southOfIstanbulStrTankersAvgSpeed = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(41.01, 41.07, 28.93, 29.09, 13);//South of Istanbul str.
                var middleOfIstanbulStrTankersAvgSpeed = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(41.07, 41.13, 28.99, 29.15, 13);//Middle of Istanbul str.
                var northOfIstanbulStrTankersAvgSpeed = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(41.13, 41.19, 29.02, 29.18, 13);//North of Istanbul str.



                var data = new
                {
                    //Çanakkale

                    southOfCanakkaleStrAllShipsCount = southOfCanakkaleStrAllShipsCount,
                    northOfCanakkaleStrAllShipsCount = northOfCanakkaleStrAllShipsCount,
                    canakkaleStraitTankersCount = southOfCanakkaleStrTankersCount + northOfCanakkaleStrTankersCount,
                    canakkaleStraitTransitShipsCount = southOfCanakkaleStrTransitShipsCount + northOfCanakkaleStrTransitShipsCount,
                    southOfCanakkaleStrAvgSpeed = southOfCanakkaleStrAvgSpeed,
                    northOfCanakkaleStrAvgSpeed = northOfCanakkaleStrAvgSpeed,
                    canakkaleStrAvgSpeed = (southOfCanakkaleStrAvgSpeed + northOfCanakkaleStrAvgSpeed) / 3,
                    canakkaleStrTankersAvgSpeed = (southOfCanakkaleStrTankersAvgSpeed + northOfCanakkaleStrTankersAvgSpeed) / 3,

                    //İstanbul

                    southOfIstanbulStrAllShipsCount = southOfIstanbulStrAllShipsCount,
                    middleOfIstanbulStrAllShipsCount = middleOfIstanbulStrAllShipsCount,
                    northOfIstanbulStrAllShipsCount = northOfIstanbulStrAllShipsCount,
                    istanbulStraitTankersCount = southOfIstanbulStrTankersCount + middleOfIstanbulStrTankersCount + northOfIstanbulStrTankersCount,
                    southOfIstanbulStrAvgSpeed = southOfIstanbulStrAvgSpeed,
                    middleOfIstanbulStrAvgSpeed = middleOfIstanbulStrAvgSpeed,
                    northOfIstanbulStrAvgSpeed = northOfIstanbulStrAvgSpeed,
                    istanbulStraitTransitShipsCount = southOfIstanbulStrTransitShipsCount + middleOfIstanbulStrTransitShipsCount + northOfIstanbulStrTransitShipsCount,
                    istanbulStrAvgSpeed = (southOfIstanbulStrAvgSpeed + middleOfIstanbulStrAvgSpeed + northOfIstanbulStrAvgSpeed) / 3,
                    southOfIstanbulStrTankersAvgSpeed = southOfIstanbulStrTankersAvgSpeed,
                    middleOfIstanbulStrTankersAvgSpeed = middleOfIstanbulStrTankersAvgSpeed,
                    northOfIstanbulStrTankersAvgSpeed = northOfIstanbulStrTankersAvgSpeed


                };

                await _hubContext.Clients.All.SendAsync("ReceiveDatas", data);

                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
