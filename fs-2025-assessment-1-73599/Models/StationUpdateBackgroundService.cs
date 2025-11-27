using fs_2025_assessment_1_73599.Models;
using fs_2025_assessment_1_73599.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class CosmosStationUpdateBackgroundService : BackgroundService
{
	private readonly CosmosStationService _cosmosService;
	private readonly ILogger<CosmosStationUpdateBackgroundService> _logger;
	private readonly Random _random = new Random();

	public CosmosStationUpdateBackgroundService(
		CosmosStationService cosmosService,
		ILogger<CosmosStationUpdateBackgroundService> logger)
	{
		_cosmosService = cosmosService;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		_logger.LogInformation("CosmosStationUpdateBackgroundService started.");

		while (!stoppingToken.IsCancellationRequested)
		{
			
			var stations = await _cosmosService.GetAllStationsAsync();

			foreach (var station in stations)
			{
				//Generate new random capacity (10–50 docks)
				int capacity = _random.Next(10, 51);

				//Generate new random available bikes (cannot exceed capacity)
				int bikes = _random.Next(0, capacity + 1);

				//Available docks are the remainder
				int docks = capacity - bikes;

				//Apply updates with invariant enforced
				station.bike_stands = capacity;
				station.available_bikes = bikes;
				station.available_bike_stands = docks;
				station.last_update = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

				//Persist to Cosmos DB
				await _cosmosService.UpdateStationAsync(station.number, station);
			}

			_logger.LogInformation("Updated {Count} stations in Cosmos DB.", stations.Count);

			//Wait 10–20 seconds before next update
			await Task.Delay(TimeSpan.FromSeconds(_random.Next(10, 21)), stoppingToken);
			
			
		}
		
	}
}
