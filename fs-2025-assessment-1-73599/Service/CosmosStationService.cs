using System.Text.Json;
using fs_2025_assessment_1_73599.Models;
using Microsoft.Azure.Cosmos;
using System.Text.Json;

namespace fs_2025_assessment_1_73599.Service
{
	public class CosmosStationService
	{
		private readonly Container _container;
		private readonly string _jsonPath;
		private List<Station>? _cachedStations;
		private bool _cosmosAvailable = true;

		public CosmosStationService(CosmosClient client, string databaseName, string containerName, string jsonPath)
		{
			_container = client.GetContainer(databaseName, containerName);
			_jsonPath = jsonPath;
		}

		/// <summary>
		/// Get stations from Cosmos DB or fallback to JSON cache
		/// </summary>
		private async Task<List<Station>> GetStationsFromSourceAsync()
		{
			if (!_cosmosAvailable)
			{
				return LoadStationsFromJson();
			}

			try
			{
				var query = _container.GetItemQueryIterator<Station>("SELECT * FROM c");
				var results = new List<Station>();
				while (query.HasMoreResults)
				{
					var response = await query.ReadNextAsync();
					results.AddRange(response);
				}
				
				if (results.Count == 0)
				{
					Console.WriteLine("[COSMOS] No stations found in Cosmos DB, falling back to JSON");
					_cosmosAvailable = false;
					return LoadStationsFromJson();
				}

				_cachedStations = results;
				return results;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[COSMOS] Error accessing Cosmos DB: {ex.Message}");
				Console.WriteLine($"[COSMOS] Exception Type: {ex.GetType().Name}");
				_cosmosAvailable = false;
				return LoadStationsFromJson();
			}
		}

		/// <summary>
		/// Load stations from JSON file
		/// </summary>
		private List<Station> LoadStationsFromJson()
		{
			try
			{
				if (_cachedStations != null)
				{
					return _cachedStations;
				}

				var json = File.ReadAllText(_jsonPath);
				var stations = JsonSerializer.Deserialize<List<Station>>(json, 
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Station>();
				
				_cachedStations = stations;
				Console.WriteLine($"[JSON CACHE] Loaded {stations.Count} stations from JSON file");
				return stations;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[JSON CACHE] Error loading JSON: {ex.Message}");
				return new List<Station>();
			}
		}

		//GET all stations
		public async Task<List<Station>> GetAllStationsAsync()
		{
			return await GetStationsFromSourceAsync();
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
	public async Task<object> QueryStationsAsync(
		string? status,
		int? minBikes,
		string? name_address,
		string? sort,
		string? acs_desc,
		int? page,
		int? pageSize)
	{
		Console.WriteLine($"[QUERY] Starting QueryStationsAsync - status:{status}, minBikes:{minBikes}, name_address:{name_address}, sort:{sort}, acs_desc:{acs_desc}, page:{page}, pageSize:{pageSize}");
		
		// Get all stations (with fallback)
		var allStations = await GetStationsFromSourceAsync();
		Console.WriteLine($"[QUERY] Retrieved {allStations.Count} total stations from source");

		// Filter by status
		if (!string.IsNullOrEmpty(status))
		{
			allStations = allStations.Where(s => s.status == status).ToList();
			Console.WriteLine($"[QUERY] After status filter: {allStations.Count} stations");
		}

		// Filter by available bikes
		if (minBikes.HasValue && minBikes > 0)
		{
			allStations = allStations.Where(s => s.available_bikes >= minBikes).ToList();
			Console.WriteLine($"[QUERY] After minBikes filter: {allStations.Count} stations");
		}

		// Search by name or address
		if (!string.IsNullOrEmpty(name_address))
		{
			var lowerSearch = name_address.ToLower();
			allStations = allStations.Where(s => 
				s.name.ToLower().Contains(lowerSearch) || 
				s.address.ToLower().Contains(lowerSearch)).ToList();
			Console.WriteLine($"[QUERY] After search filter: {allStations.Count} stations");
		}

		// Store total count BEFORE sorting and pagination
		int totalCount = allStations.Count;
		Console.WriteLine($"[QUERY] Total filtered stations: {totalCount}");

		// Sorting
		if (!string.IsNullOrEmpty(sort))
		{
			bool ascending = string.Equals(acs_desc, "asc", StringComparison.OrdinalIgnoreCase);
			Console.WriteLine($"[QUERY] Sorting by {sort} ({(ascending ? "ASC" : "DESC")})");

			allStations = sort.ToLower() switch
			{
				"name" => ascending 
					? allStations.OrderBy(s => s.name).ToList()
					: allStations.OrderByDescending(s => s.name).ToList(),
				"availablebikes" => ascending
					? allStations.OrderBy(s => s.available_bikes).ToList()
					: allStations.OrderByDescending(s => s.available_bikes).ToList(),
				"occupancy" => ascending
					? allStations.OrderBy(s => s.bike_stands > 0 ? (s.available_bikes * 100.0 / s.bike_stands) : 0).ToList()
					: allStations.OrderByDescending(s => s.bike_stands > 0 ? (s.available_bikes * 100.0 / s.bike_stands) : 0).ToList(),
				_ => allStations
			};
			Console.WriteLine($"[QUERY] After sorting: {allStations.Count} stations");
		}

		// Pagination (manual)
		List<Station> paginatedResults = allStations;
		if (page.HasValue && pageSize.HasValue && page > 0 && pageSize > 0)
		{
			int skip = (page.Value - 1) * pageSize.Value;
			paginatedResults = allStations
				.Skip(skip)
				.Take(pageSize.Value)
				.ToList();
			Console.WriteLine($"[QUERY] After pagination (page {page}, size {pageSize}): {paginatedResults.Count} stations");
		}

		Console.WriteLine($"[QUERY] Returning {paginatedResults.Count} stations out of {totalCount} total");
		
		// Return both the paginated data and total count
		return new
		{
			data = paginatedResults,
			total = totalCount,
			page = page ?? 1,
			pageSize = pageSize ?? 10
		};
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
