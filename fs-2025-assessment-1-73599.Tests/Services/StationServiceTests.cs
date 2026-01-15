using fs_2025_assessment_1_73599.Models;
using fs_2025_assessment_1_73599.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace fs_2025_assessment_1_73599.Tests.Services
{
	public class StationServiceTests
	{
		private readonly string _testJsonPath = Path.Combine(Path.GetTempPath(), "test_stations.json");
		private List<Station> CreateTestStations()
		{
			return new List<Station>
			{
				new Station
				{
					number = 1,
					name = "Station A",
					address = "123 Main St",
					available_bikes = 10,
					bike_stands = 20,
					status = "OPEN",
					contract_name = "Dublin"
				},
				new Station
				{
					number = 2,
					name = "Station B",
					address = "456 Oak Ave",
					available_bikes = 5,
					bike_stands = 15,
					status = "CLOSED",
					contract_name = "Dublin"
				},
				new Station
				{
					number = 3,
					name = "Station C",
					address = "789 Pine Rd",
					available_bikes = 15,
					bike_stands = 25,
					status = "OPEN",
					contract_name = "Dublin"
				}
			};
		}

		[Fact]
		public void GetAllStations_ReturnsAllStations()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.GetAllStations();
			
			Assert.Equal(3, result.Count);
			Assert.Equal("Station A", result[0].name);
		}

		[Fact]
		public void GetStationByNumber_WithValidNumber_ReturnsStation()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.GetStationByNumber(2);
			
			Assert.NotNull(result);
			Assert.Equal("Station B", result.name);
			Assert.Equal(2, result.number);
		}

		[Fact]
		public void GetStationByNumber_WithInvalidNumber_ReturnsNull()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.GetStationByNumber(999);
			
			Assert.Null(result);
		}

		[Fact]
		public void QueryStations_FilterByStatus_ReturnsFilteredStations()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.QueryStations(status: "OPEN", minBikes: null, name_address: null, sort: null, acs_desc: null, page: null, pageSize: null).ToList();
			
			Assert.Equal(2, result.Count);
			Assert.All(result, s => Assert.Equal("OPEN", s.status));
		}

		[Fact]
		public void QueryStations_FilterByMinBikes_ReturnsStationsWithEnoughBikes()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.QueryStations(status: null, minBikes: 10, name_address: null, sort: null, acs_desc: null, page: null, pageSize: null).ToList();
			
			Assert.Equal(2, result.Count);
			Assert.All(result, s => Assert.True(s.available_bikes >= 10));
		}

		[Fact]
		public void QueryStations_SearchByName_ReturnsMatchingStations()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.QueryStations(status: null, minBikes: null, name_address: "Station A", sort: null, acs_desc: null, page: null, pageSize: null).ToList();
			
			Assert.Single(result);
			Assert.Equal("Station A", result[0].name);
		}

		[Fact]
		public void QueryStations_SearchByAddress_ReturnsMatchingStations()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.QueryStations(status: null, minBikes: null, name_address: "Oak", sort: null, acs_desc: null, page: null, pageSize: null).ToList();
			
			Assert.Single(result);
			Assert.Equal("Station B", result[0].name);
		}

		[Fact]
		public void QueryStations_SortByNameAscending_ReturnsSortedStations()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.QueryStations(status: null, minBikes: null, name_address: null, sort: "name", acs_desc: "asc", page: null, pageSize: null).ToList();
			
			Assert.Equal(3, result.Count);
			Assert.Equal("Station A", result[0].name);
			Assert.Equal("Station B", result[1].name);
			Assert.Equal("Station C", result[2].name);
		}

		[Fact]
		public void QueryStations_SortByNameDescending_ReturnsSortedStations()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.QueryStations(status: null, minBikes: null, name_address: null, sort: "name", acs_desc: "desc", page: null, pageSize: null).ToList();
			
			Assert.Equal(3, result.Count);
			Assert.Equal("Station C", result[0].name);
			Assert.Equal("Station B", result[1].name);
			Assert.Equal("Station A", result[2].name);
		}

		[Fact]
		public void QueryStations_SortByAvailableBikes_ReturnsSortedStations()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.QueryStations(status: null, minBikes: null, name_address: null, sort: "availablebikes", acs_desc: "asc", page: null, pageSize: null).ToList();
			
			Assert.Equal(3, result.Count);
			Assert.Equal(5, result[0].available_bikes);
			Assert.Equal(10, result[1].available_bikes);
			Assert.Equal(15, result[2].available_bikes);
		}

		[Fact]
		public void QueryStations_SortByOccupancy_ReturnsSortedStations()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.QueryStations(status: null, minBikes: null, name_address: null, sort: "occupancy", acs_desc: "asc", page: null, pageSize: null).ToList();
			
			Assert.Equal(3, result.Count);
			Assert.Equal("Station B", result[0].name);
			Assert.Equal("Station A", result[1].name);
			Assert.Equal("Station C", result[2].name);
		}

		[Fact]
		public void QueryStations_WithPagination_ReturnsPaginatedStations()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.QueryStations(status: null, minBikes: null, name_address: null, sort: null, acs_desc: null, page: 2, pageSize: 1).ToList();
			
			Assert.Single(result);
			Assert.Equal("Station B", result[0].name);
		}

		[Fact]
		public void QueryStations_WithPaginationFirstPage_ReturnsFirstPageStations()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.QueryStations(status: null, minBikes: null, name_address: null, sort: null, acs_desc: null, page: 1, pageSize: 2).ToList();
			
			Assert.Equal(2, result.Count);
			Assert.Equal("Station A", result[0].name);
			Assert.Equal("Station B", result[1].name);
		}

		[Fact]
		public void GetSummary_ReturnsSummaryData()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.GetSummary();
			
			Assert.NotNull(result);
			var properties = result.GetType().GetProperties();
			Assert.Contains(properties, p => p.Name == "totalStations");
			Assert.Contains(properties, p => p.Name == "totalBikeStands");
			Assert.Contains(properties, p => p.Name == "totalAvailableBikes");
			Assert.Contains(properties, p => p.Name == "statusCounts");
		}

		[Fact]
		public void AddStation_AddsNewStationToList()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var newStation = new Station
			{
				number = 4,
				name = "Station D",
				address = "999 New St",
				available_bikes = 20,
				bike_stands = 30,
				status = "OPEN",
				contract_name = "Dublin"
			};

			service.AddStation(newStation);
			
			Assert.Equal(4, service.GetAllStations().Count);
			var added = service.GetStationByNumber(4);
			Assert.NotNull(added);
			Assert.Equal("Station D", added.name);
		}

		[Fact]
		public void UpdateStation_UpdatesExistingStation()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var updated = new Station
			{
				number = 1,
				name = "Updated Station A",
				address = "123 Updated St",
				available_bikes = 20,
				bike_stands = 30,
				status = "CLOSED",
				contract_name = "Dublin"
			};

			var result = service.UpdateStation(1, updated);
			
			Assert.True(result);
			var station = service.GetStationByNumber(1);
			Assert.NotNull(station);
			Assert.Equal("Updated Station A", station.name);
			Assert.Equal("123 Updated St", station.address);
			Assert.Equal("CLOSED", station.status);
		}

		[Fact]
		public void UpdateStation_WithInvalidNumber_ReturnsFalse()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var updated = new Station { number = 999, name = "Fake Station" };
			var result = service.UpdateStation(999, updated);
			
			Assert.False(result);
		}

		[Fact]
		public void QueryStations_CombineMultipleFilters_ReturnsCorrectResults()
		{
			var stations = CreateTestStations();
			var service = new StationService(stations, _testJsonPath);
			var result = service.QueryStations(
				status: "OPEN",
				minBikes: 10,
				name_address: null,
				sort: null,
				acs_desc: null,
				page: null,
				pageSize: null).ToList();
			
			Assert.Equal(2, result.Count);
			Assert.All(result, s => Assert.Equal("OPEN", s.status));
			Assert.All(result, s => Assert.True(s.available_bikes >= 10));
		}
	}
}
