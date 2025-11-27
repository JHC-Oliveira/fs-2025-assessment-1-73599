using System.Text.Json;
using fs_2025_assessment_1_73599.Models;

namespace fs_2025_assessment_1_73599.Service
{
	public class StationService
	{
		private readonly List<Station> _stations;
		private readonly string _jsonPath;

		// Constructor to initialize the service with a list of stations
		public StationService(List<Station> stations, string jsonPath)
		{
			_stations = stations; 
			_jsonPath = jsonPath;
		}

		public List<Station> GetAllStations() => _stations;

		// Get station by its unique number
		public Station? GetStationByNumber(int number) =>
			_stations.FirstOrDefault(s => s.number == number);

		// Query for GET /api/stations
		public IEnumerable<Station> QueryStations(
			string? status,
			int? minBikes,
			string? name_address,
			string? sort,
			string? acs_desc,
			int? page,
			int? pageSize)
		{
			IEnumerable<Station> query = _stations;

			// Filter by status
			if (!string.IsNullOrEmpty(status))
			{
				query = query.Where(s => s.status.Equals(status, StringComparison.OrdinalIgnoreCase));
			}

			// Filter by minimum available bikes
			if (minBikes.HasValue)
			{
				query = query.Where(s => s.available_bikes >= minBikes.Value);
			}

			// Search by name or address
			if (!string.IsNullOrEmpty(name_address))
			{
				query = query.Where(s =>
					s.name.Contains(name_address, StringComparison.OrdinalIgnoreCase) ||
					s.address.Contains(name_address, StringComparison.OrdinalIgnoreCase));
			}

			// Sorting
			if (!string.IsNullOrEmpty(sort))
			{
				bool ascending = string.Equals(acs_desc, "asc", StringComparison.OrdinalIgnoreCase);

				query = sort.ToLower() switch
				{
					"name" => ascending ? query.OrderBy(s => s.name) : query.OrderByDescending(s => s.name),
					"availablebikes" => ascending ? query.OrderBy(s => s.available_bikes) : query.OrderByDescending(s => s.available_bikes),
					"occupancy" => ascending
						? query.OrderBy(s => (double)s.available_bikes / s.bike_stands)
						: query.OrderByDescending(s => (double)s.available_bikes / s.bike_stands),
					_ => query
				};
			}

			// Pagination
			if (page.HasValue && pageSize.HasValue && page > 0 && pageSize > 0)
			{
				query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
			}

			return query;
		}

		public object GetSummary()
		{
			return new
			{
				totalStations = _stations.Count,
				totalBikeStands = _stations.Sum(s => s.bike_stands),
				totalAvailableBikes = _stations.Sum(s => s.available_bikes),
				statusCounts = _stations.GroupBy(s => s.status)
										.ToDictionary(g => g.Key, g => g.Count())
			};
		}
		
		public void AddStation(Station station)
		{
			_stations.Add(station);
			SaveStations();
		}

		public bool UpdateStation(int number, Station updated)
		{
			var station = GetStationByNumber(number);
			if (station == null) return false;

			station.name = updated.name;
			station.address = updated.address;
			station.bike_stands = updated.bike_stands;
			station.available_bikes = updated.available_bikes;
			station.status = updated.status;
			SaveStations();

			return true;
		}

		//Method to persist changes back to JSON file
		private void SaveStations()
		{
			var options = new JsonSerializerOptions { WriteIndented = true };
			var json = JsonSerializer.Serialize(_stations, options);
			File.WriteAllText(_jsonPath, json);
		}

	}
}
