using fs_2025_assessment_1_73599_blazorapp.Models;
using System.Text;
using System.Text.Json;

namespace fs_2025_assessment_1_73599_blazorapp.Services
{
	public class StationsApiClient
	{
		private readonly HttpClient _httpClient;
		private const string ApiV2Base = "/api/v2/stations";

		public StationsApiClient(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		/// Get a paginated and filtered list of stations
		public async Task<StationResponse> GetStationsAsync(
			int page = 1,
			int pageSize = 10,
			string searchTerm = "",
			string status = "",
			int minBikes = 0,
			string sortBy = "",
			string sortDirection = "asc")
		{
			try
			{
				var queryParams = new List<string>();
				queryParams.Add($"page={page}");
				queryParams.Add($"pageSize={pageSize}");

				if (!string.IsNullOrEmpty(searchTerm))
					queryParams.Add($"name_address={Uri.EscapeDataString(searchTerm)}");

				if (!string.IsNullOrEmpty(status))
					queryParams.Add($"status={Uri.EscapeDataString(status)}");

				if (minBikes > 0)
					queryParams.Add($"minBikes={minBikes}");

				if (!string.IsNullOrEmpty(sortBy))
				{
					queryParams.Add($"sort={Uri.EscapeDataString(sortBy)}");
					queryParams.Add($"acs_desc={Uri.EscapeDataString(sortDirection)}");
				}

				var queryString = string.Join("&", queryParams);
				var url = $"{ApiV2Base}/query?{queryString}";

				Console.WriteLine($"[API CLIENT] Fetching from: {url}");
				Console.WriteLine($"[API CLIENT] Parameters - Page: {page}, PageSize: {pageSize}, Search: {searchTerm}, Status: {status}, MinBikes: {minBikes}, Sort: {sortBy}, Direction: {sortDirection}");

				var response = await _httpClient.GetAsync(url);
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"[API CLIENT] Response status: {response.StatusCode}");
				Console.WriteLine($"[API CLIENT] Response length: {content.Length} characters");
				Console.WriteLine($"[API CLIENT] Response preview: {content.Substring(0, Math.Min(500, content.Length))}");

				// API now returns an object with data and total
				using var jsonDoc = JsonDocument.Parse(content);
				var root = jsonDoc.RootElement;

				// Extract the data array
				var dataArray = root.GetProperty("data");
				var stationsList = JsonSerializer.Deserialize<List<Station>>(dataArray.GetRawText(),
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<Station>();

				// Extract the total count
				int total = root.GetProperty("total").GetInt32();

				Console.WriteLine($"[API CLIENT] Deserialized {stationsList.Count} stations, total: {total}");

				if (stationsList.Count == 0)
				{
					Console.WriteLine($"[API CLIENT] Returning empty response");
					return new StationResponse { data = new(), total = total, page = page, pageSize = pageSize };
				}

				// Wrap the response to match our StationResponse model
				var result = new StationResponse
				{
					data = stationsList,
					total = total,
					page = page,
					pageSize = pageSize
				};
				Console.WriteLine($"[API CLIENT] Returning {result.data.Count} stations with total {result.total}");
				return result;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[API CLIENT] ERROR fetching stations: {ex.Message}");
				Console.WriteLine($"[API CLIENT] Exception Type: {ex.GetType().Name}");
				Console.WriteLine($"[API CLIENT] Stack trace: {ex.StackTrace}");
				if (ex.InnerException != null)
				{
					Console.WriteLine($"[API CLIENT] Inner Exception: {ex.InnerException.Message}");
				}
				return new StationResponse();
			}
		}

		/// Get a single station by number
		public async Task<Station?> GetStationByNumberAsync(int number)
		{
			try
			{
				var url = $"{ApiV2Base}/{number}";
				Console.WriteLine($"[API CLIENT] Fetching station from: {url}");
				
				var response = await _httpClient.GetAsync(url);
				
				if (!response.IsSuccessStatusCode)
				{
					Console.WriteLine($"[API CLIENT] Station {number} not found (Status: {response.StatusCode})");
					return null;
				}

				var content = await response.Content.ReadAsStringAsync();
				Console.WriteLine($"[API CLIENT] Station response preview: {content.Substring(0, Math.Min(300, content.Length))}");
				
				var station = JsonSerializer.Deserialize<Station>(content,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				if (station != null)
				{
					Console.WriteLine($"[API CLIENT] Station {number} loaded: {station.name}");
				}
				return station;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"[API CLIENT] Error fetching station {number}: {ex.Message}");
				Console.WriteLine($"[API CLIENT] Stack trace: {ex.StackTrace}");
				return null;
			}
		}

		/// Get summary information about all stations
		public async Task<SummaryResponse> GetSummaryAsync()
		{
			try
			{
				var url = $"{ApiV2Base}/summary";
				var response = await _httpClient.GetAsync(url);
				response.EnsureSuccessStatusCode();

				var content = await response.Content.ReadAsStringAsync();
				var summary = JsonSerializer.Deserialize<SummaryResponse>(content,
					new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

				return summary ?? new SummaryResponse();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error fetching summary: {ex.Message}");
				return new SummaryResponse();
			}
		}

		/// Create a new station
		public async Task<bool> CreateStationAsync(Station station)
		{
			try
			{
				var json = JsonSerializer.Serialize(station);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var response = await _httpClient.PostAsync(ApiV2Base, content);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error creating station: {ex.Message}");
				return false;
			}
		}

		/// Update an existing station
		public async Task<bool> UpdateStationAsync(int number, Station station)
		{
			try
			{
				var json = JsonSerializer.Serialize(station);
				var content = new StringContent(json, Encoding.UTF8, "application/json");
				var url = $"{ApiV2Base}/{number}";
				var response = await _httpClient.PutAsync(url, content);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error updating station {number}: {ex.Message}");
				return false;
			}
		}

		/// Delete a station
		public async Task<bool> DeleteStationAsync(int number)
		{
			try
			{
				var url = $"{ApiV2Base}/{number}";
				var response = await _httpClient.DeleteAsync(url);
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error deleting station {number}: {ex.Message}");
				return false;
			}
		}
	}
}