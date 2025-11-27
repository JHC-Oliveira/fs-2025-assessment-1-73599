using Microsoft.Azure.Cosmos;
using System.Text.Json;
using fs_2025_assessment_1_73599.Models;

namespace fs_2025_assessment_1_73599.Seeding
{
	public class CosmosSeeder
	{
		private readonly Container _container;

		public CosmosSeeder(CosmosClient client, string databaseName, string containerName)
		{
			_container = client.GetContainer(databaseName, containerName);
		}

		public async Task SeedFromJsonAsync(string jsonPath)
		{
			var json = await File.ReadAllTextAsync(jsonPath);
			var stations = JsonSerializer.Deserialize<List<Station>>(json) ?? new List<Station>();
			Console.WriteLine($"📄 Reading from: {jsonPath}");
			Console.WriteLine($"📄 Found {stations.Count} stations");


			foreach (var station in stations)
			{
				
				try
				{  					
					await _container.UpsertItemAsync(station, new PartitionKey(station.number));
					Console.WriteLine(JsonSerializer.Serialize(station));
					Console.WriteLine($"Seeded station {station.number}");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error seeding station {station.number}: {ex.Message}");
				}
			}
		}
	}
}
