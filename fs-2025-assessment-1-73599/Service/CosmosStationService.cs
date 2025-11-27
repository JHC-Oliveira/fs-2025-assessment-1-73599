using System.Text.Json;
using fs_2025_assessment_1_73599.Models;
using Microsoft.Azure.Cosmos;

namespace fs_2025_assessment_1_73599.Service
{
	public class CosmosStationService
	{
		private readonly Container _container;
		private readonly string _jsonPath;

		public CosmosStationService(CosmosClient client, string databaseName, string containerName, string jsonPath)
		{
			_container = client.GetContainer(databaseName, containerName);
			_jsonPath = jsonPath;
		}

		//GET all stations
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

		//GET station by number
		public async Task<Station?> GetStationByNumberAsync(int number)
		{
			try
			{
				ItemResponse<Station> response = await _container.ReadItemAsync<Station>(
					number.ToString(), new PartitionKey(number));
				return response.Resource;
			}
			catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
			{
				return null;
			}
		}

		//Create a new station
		public async Task AddStationAsync(Station station)
		{
			await _container.CreateItemAsync(station, new PartitionKey(station.number));
			await SaveStationsToJsonAsync();
		}

		//Update a station
		public async Task<bool> UpdateStationAsync(int number, Station updated)
		{
			var existing = await GetStationByNumberAsync(number);
			if (existing == null) return false;

			// overwrite fields
			existing.name = updated.name;
			existing.address = updated.address;
			existing.bike_stands = updated.bike_stands;
			existing.available_bike_stands = updated.available_bike_stands;
			existing.available_bikes = updated.available_bikes;
			existing.status = updated.status;
			existing.last_update = updated.last_update;

			await _container.UpsertItemAsync(existing, new PartitionKey(number));
			await SaveStationsToJsonAsync();
			return true;
		}

		private async Task SaveStationsToJsonAsync()
		{
			var stations = await GetAllStationsAsync();
			var options = new JsonSerializerOptions { WriteIndented = true };
			var json = JsonSerializer.Serialize(stations, options);
			File.WriteAllText(_jsonPath, json);
		}

		//Query filter stations
		public async Task<List<Station>> QueryStationsAsync(
			string? status,
			int? minBikes,
			string? name_address,
			string? sort,
			string? acs_desc,
			int? page,
			int? pageSize)
		{
			var sql = "SELECT * FROM c WHERE 1=1";

			// Filter by status
			if (!string.IsNullOrEmpty(status))
				sql += " AND c.status = @status";

			// Filter by available bikes
			if (minBikes.HasValue)
				sql += " AND c.available_bikes >= @minBikes";

			// Search by name or address
			if (!string.IsNullOrEmpty(name_address))
				sql += " AND (CONTAINS(LOWER(c.name), @kw) OR CONTAINS(LOWER(c.address), @kw))";

			// Sorting
			if (!string.IsNullOrEmpty(sort))
			{
				bool ascending = string.Equals(acs_desc, "asc", StringComparison.OrdinalIgnoreCase);
				string direction = ascending ? "ASC" : "DESC";

				sql += sort.ToLower() switch
				{
					"name" => $" ORDER BY c.name {direction}",
					"availablebikes" => $" ORDER BY c.available_bikes {direction}",
					"occupancy" => $" ORDER BY (c.available_bikes * 1.0 / c.bike_stands) {direction}",
					_ => ""
				};
			}

			var queryDef = new QueryDefinition(sql);

			if (!string.IsNullOrEmpty(status))
				queryDef.WithParameter("@status", status);

			if (minBikes.HasValue)
				queryDef.WithParameter("@minBikes", minBikes.Value);

			if (!string.IsNullOrEmpty(name_address))
				queryDef.WithParameter("@kw", name_address.ToLower());

			var query = _container.GetItemQueryIterator<Station>(queryDef);
			var results = new List<Station>();
			while (query.HasMoreResults)
			{
				var response = await query.ReadNextAsync();
				results.AddRange(response);
			}

			// Pagination (manual)
			if (page.HasValue && pageSize.HasValue && page > 0 && pageSize > 0)
			{
				results = results
					.Skip((page.Value - 1) * pageSize.Value)
					.Take(pageSize.Value)
					.ToList();
			}

			return results;

		}

		public async Task<object> GetSummaryAsync()
		{
			var stations = await GetAllStationsAsync();

			return new
			{
				totalStations = stations.Count,
				totalBikeStands = stations.Sum(s => s.bike_stands),
				totalAvailableBikes = stations.Sum(s => s.available_bikes),
				statusCounts = stations
					.GroupBy(s => s.status)
					.ToDictionary(g => g.Key, g => g.Count())
			};
		}

	}
}
