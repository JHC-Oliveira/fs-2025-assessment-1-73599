using System.Text.Json;
using fs_2025_assessment_1_73599.Models;
using fs_2025_assessment_1_73599.Service;
using Microsoft.Azure.Cosmos;

namespace fs_2025_assessment_1_73599.Startup
{
	public static class ServiceExtensions
	{
		//V1: JSON file + in-memory
		public static void AddStationService(this IServiceCollection services, string jsonPath)
		{
			var json = File.ReadAllText(jsonPath);
			var stations = JsonSerializer.Deserialize<List<Station>>(json) ?? new List<Station>();

			services.AddSingleton(new StationService(stations, jsonPath));
		}

		// V2: Cosmos DB
		public static void AddCosmosStationService(this IServiceCollection services, IConfiguration config, string jsonPath)
		{
			// Read settings from appsettings.json
			var endpointUri = config["CosmosDb:EndpointUri"];
			var primaryKey = config["CosmosDb:PrimaryKey"];
			var databaseName = config["CosmosDb:DatabaseName"];
			var containerName = config["CosmosDb:ContainerName"];


			var cosmosClient = new CosmosClient(endpointUri, primaryKey);
			services.AddSingleton(new CosmosStationService(cosmosClient, databaseName, containerName, jsonPath));
		}

		// Background updater (V2 sync)
		public static void AddStationUpdater(this IServiceCollection services)
		{
			services.AddHostedService<CosmosStationUpdateBackgroundService>();
		}

	}
}
