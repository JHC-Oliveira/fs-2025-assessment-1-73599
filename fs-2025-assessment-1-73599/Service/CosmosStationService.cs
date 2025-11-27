using fs_2025_assessment_1_73599.Models;
using Microsoft.Azure.Cosmos;


namespace fs_2025_assessment_1_73599.Service
{
	public class CosmosStationService
	{
		private readonly Container _container;

		public CosmosStationService(CosmosClient client, string databaseName, string containerName)
		{
			_container = client.GetContainer(databaseName, containerName);
		}

		public async Task<List<Station>> GetAllStationsAsync()
		{
			var query = _container.GetItemQueryIterator<Station>("SELECT * FROM c");
			var results = new List<Station>();
			while (query.HasMoreResults)
			{
				var response = await query.ReadNextAsync();
				results.AddRange(response);
			}
			return results;
		}

		public async Task<Station?> GetStationByNumberAsync(int number)
		{
			try
			{
				ItemResponse<Station> response = await _container.ReadItemAsync<Station>(
					number.ToString(), new PartitionKey(number.ToString()));
				return response.Resource;
			}
			catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				return null;
			}
		}

		public async Task AddStationAsync(Station station)
		{
			await _container.CreateItemAsync(station, new PartitionKey(station.number.ToString()));
		}

		public async Task<bool> UpdateStationAsync(int number, Station updated)
		{
			var existing = await GetStationByNumberAsync(number);
			if (existing == null) return false;

			// overwrite fields
			existing.name = updated.name;
			existing.address = updated.address;
			existing.bike_stands = updated.bike_stands;
			existing.available_bikes = updated.available_bikes;
			existing.status = updated.status;
			existing.last_update = updated.last_update;

			await _container.UpsertItemAsync(existing, new PartitionKey(number.ToString()));
			return true;
		}
	}
}
