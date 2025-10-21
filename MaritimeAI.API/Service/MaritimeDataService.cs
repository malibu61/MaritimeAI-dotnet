using MaritimeAI.API.Hubs;
using MaritimeAI.BusinessLayer.Abstract;
using Microsoft.AspNetCore.SignalR;

namespace MaritimeAI.API.Service
{
    public class MaritimeDataService : BackgroundService
    {
        private readonly IHubContext<MaritimeHub> _hubContext;
        private readonly IShipsService _shipsService;
        private readonly ILogger<MaritimeDataService> _logger;

        private object? _lastSuccessfulData = null;

        public MaritimeDataService(IHubContext<MaritimeHub> hubContext, IShipsService shipsService, ILogger<MaritimeDataService> logger)
        {
            _hubContext = hubContext;
            _shipsService = shipsService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    //Çanakkale
                    var southOfCanakkaleStrAllShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);//South of Canakkale str.
                    await Task.Delay(100, stoppingToken);
                    var northOfCanakkaleStrAllShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);//North of Canakkale str.
                    await Task.Delay(100, stoppingToken);


                    var southOfCanakkaleStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);//South of Canakkale str.
                    await Task.Delay(100, stoppingToken);
                    var northOfCanakkaleStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);//North of Canakkale str.
                    await Task.Delay(100, stoppingToken);


                    var southOfCanakkaleStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);//South of Canakkale str.
                    await Task.Delay(100, stoppingToken);
                    var northOfCanakkaleStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);//North of Canakkale str.
                    await Task.Delay(100, stoppingToken);


                    var southOfCanakkaleStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);//South of Canakkale str.
                    await Task.Delay(100, stoppingToken);
                    var northOfCanakkaleStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);//North of Canakkale str.
                    await Task.Delay(100, stoppingToken);


                    var southOfCanakkaleStrTankersAvgSpeed = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(39.91, 40.15, 26.18, 26.82, 11);//South of Canakkale str.
                    await Task.Delay(100, stoppingToken);
                    var northOfCanakkaleStrTankersAvgSpeed = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(40.15, 40.40, 26.34, 26.98, 11);//North of Canakkale str.
                    await Task.Delay(100, stoppingToken);



                    //İstanbul

                    var southOfIstanbulStrAllShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);//South of Istanbul str.
                    await Task.Delay(100, stoppingToken);
                    var middleOfIstanbulStrAllShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);//Middle of Istanbul str.
                    await Task.Delay(100, stoppingToken);
                    var northOfIstanbulStrAllShipsCount = await _shipsService.GetShipsCountByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);//North of Istanbul str.
                    await Task.Delay(100, stoppingToken);


                    var southOfIstanbulStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);//South of Istanbul str.
                    await Task.Delay(100, stoppingToken);
                    var middleOfIstanbulStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);//Middle of Istanbul str.
                    await Task.Delay(100, stoppingToken);
                    var northOfIstanbulStrTankersCount = await _shipsService.GetTankersCountByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);//North of Istanbul str.
                    await Task.Delay(100, stoppingToken);


                    var southOfIstanbulStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);//South of Istanbul str.
                    await Task.Delay(100, stoppingToken);
                    var middleOfIstanbulStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);//Middle of Istanbul str.
                    await Task.Delay(100, stoppingToken);
                    var northOfIstanbulStrAvgSpeed = await _shipsService.GetShipsAvgSpeedByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);//North of Istanbul str.
                    await Task.Delay(100, stoppingToken);


                    var southOfIstanbulStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);//South of Istanbul str.
                    await Task.Delay(100, stoppingToken);
                    var middleOfIstanbulStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);//Middle of Istanbul str.
                    await Task.Delay(100, stoppingToken);
                    var northOfIstanbulStrTransitShipsCount = await _shipsService.GetTransitShipsCountByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);//North of Istanbul str.
                    await Task.Delay(100, stoppingToken);


                    var southOfIstanbulStrTankersAvgSpeed = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(40.99, 41.078, 28.92, 29.146, 12);//South of Istanbul str.
                    await Task.Delay(100, stoppingToken);
                    var middleOfIstanbulStrTankersAvgSpeed = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(41.078, 41.16, 28.917, 29.141, 12);//Middle of Istanbul str.
                    await Task.Delay(100, stoppingToken);
                    var northOfIstanbulStrTankersAvgSpeed = await _shipsService.GetTankersAvgSpeedByCoordinatesAsync(41.16, 41.242, 28.965, 29.189, 12);//North of Istanbul str.
                    await Task.Delay(100, stoppingToken);



                    var data = new
                    {
                        //Çanakkale

                        southOfCanakkaleStrAllShipsCount = southOfCanakkaleStrAllShipsCount,
                        northOfCanakkaleStrAllShipsCount = northOfCanakkaleStrAllShipsCount,
                        canakkaleStraitTankersCount = southOfCanakkaleStrTankersCount + northOfCanakkaleStrTankersCount,
                        canakkaleStraitTransitShipsCount = southOfCanakkaleStrTransitShipsCount + northOfCanakkaleStrTransitShipsCount,
                        southOfCanakkaleStrAvgSpeed = southOfCanakkaleStrAvgSpeed,
                        northOfCanakkaleStrAvgSpeed = northOfCanakkaleStrAvgSpeed,
                        canakkaleStrAvgSpeed = (southOfCanakkaleStrAvgSpeed + northOfCanakkaleStrAvgSpeed) / 2,
                        canakkaleStrTankersAvgSpeed = (southOfCanakkaleStrTankersAvgSpeed + northOfCanakkaleStrTankersAvgSpeed) / 2,

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

                    _lastSuccessfulData = data;

                    await _hubContext.Clients.All.SendAsync("ReceiveDatas", data);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Veri alınırken hata oluştu.");

                    if (_lastSuccessfulData != null)
                    {
                        await _hubContext.Clients.All.SendAsync("ReceiveDatas", _lastSuccessfulData);
                    }
                }

                await Task.Delay(2000, stoppingToken);
            }
        }
    }
}