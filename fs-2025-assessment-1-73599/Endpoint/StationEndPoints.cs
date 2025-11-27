using fs_2025_assessment_1_73599.Models;
using fs_2025_assessment_1_73599.Service;

namespace fs_2025_assessment_1_73599.Endpoint
{
	public static class StationEndPoints
	{
		public static void MapStationEndpoints(this WebApplication app)
		{
			// ---------------- V1 ENDPOINTS (JSON file) ----------------
			// GET all stations with filters, search, sort, pagination
			app.MapGet("/api/stations", (
				string? status,
				int? minBikes,
				string? name_address,
				string? sort,
				string? asc_desc,
				int? page,
				int? pageSize,
				StationService stationService) =>
			{
				var results = stationService.QueryStations(status, minBikes, name_address, sort, asc_desc, page, pageSize);
				return results.Any() ? Results.Ok(results) : Results.NotFound(new { message = "No stations found" });
			});


			// GET station by number
			app.MapGet("/api/stations/{number:int}", (int number, StationService stationService) =>
			{
				var station = stationService.GetStationByNumber(number);
				return station is not null ? Results.Ok(station) : Results.NotFound();
			});
			

			// GET summary
			app.MapGet("/api/stations/summary", (StationService stationService) =>
			{
				return Results.Ok(stationService.GetSummary());
			});

			// CREATE station
			app.MapPost("/api/stations", (Station station, StationService stationService) =>
			{
				stationService.AddStation(station);
				return Results.Created($"/api/stations/{station.number}", station);
			});

			// PUT update existing station
			app.MapPut("/api/stations/{number:int}", (int number, Station updated, StationService stationService) =>
			{
				var success = stationService.UpdateStation(number, updated);
				return success ? Results.Ok(updated) : Results.NotFound(new { message = $"Station {number} not found" });
			});

			// ---------------- V2 ENDPOINTS (Cosmos DB) ----------------

			// GET all stations
			app.MapGet("/api/v2/stations", async (CosmosStationService cosmosService) =>
			{
				var stations = await cosmosService.GetAllStationsAsync();
				return stations.Any() ? Results.Ok(stations) : Results.NotFound(new { message = "No stations found" });
			});

			// GET station by number
			app.MapGet("/api/v2/stations/{number:int}", async (int number, CosmosStationService cosmosService) =>
			{
				var station = await cosmosService.GetStationByNumberAsync(number);
				return station is not null ? Results.Ok(station) : Results.NotFound();
			});

			// CREATE station
			app.MapPost("/api/v2/stations", async (Station station, CosmosStationService cosmosService) =>
			{
				await cosmosService.AddStationAsync(station);
				return Results.Created($"/api/v2/stations/{station.number}", station);
			});

			// UPDATE station
			app.MapPut("/api/v2/stations/{number:int}", async (int number, Station updated, CosmosStationService cosmosService) =>
			{
				var success = await cosmosService.UpdateStationAsync(number, updated);
				return success ? Results.Ok(updated) : Results.NotFound(new { message = $"Station {number} not found" });
			});			

		}
	}
}
