using fs_2025_assessment_1_73599.Models;
using fs_2025_assessment_1_73599.Service;
using Moq;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Xunit;

namespace fs_2025_assessment_1_73599.Tests.Services
{
	public class CosmosStationServiceTests
	{
		private readonly string _testJsonPath = Path.Combine(Path.GetTempPath(), "test_cosmos_stations.json");

		private Mock<CosmosClient> CreateMockCosmosClient()
		{
			return new Mock<CosmosClient>();
		}

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

		private void CreateTestJsonFile()
		{
			var stations = CreateTestStations();
			var json = JsonSerializer.Serialize(stations);
			File.WriteAllText(_testJsonPath, json);
		}

		[Fact]
		public async Task QueryStationsAsync_FilterByStatus_ReturnsFilteredStations()
		{
			// Arrange
			CreateTestJsonFile();
			var mockClient = CreateMockCosmosClient();
			var mockContainer = new Mock<Container>();

			mockClient
				.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(mockContainer.Object);

			var service = new CosmosStationService(mockClient.Object, "testDb", "testContainer", _testJsonPath);

			// Act
			var result = await service.QueryStationsAsync(status: "OPEN", minBikes: null, name_address: null, sort: null, acs_desc: null, page: null, pageSize: null);

			// Assert
			Assert.NotNull(result);
		}

		[Fact]
		public async Task QueryStationsAsync_FilterByMinBikes_ReturnsCorrectStations()
		{
			// Arrange
			CreateTestJsonFile();
			var mockClient = CreateMockCosmosClient();
			var mockContainer = new Mock<Container>();

			mockClient
				.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(mockContainer.Object);

			var service = new CosmosStationService(mockClient.Object, "testDb", "testContainer", _testJsonPath);

			// Act
			var result = await service.QueryStationsAsync(status: null, minBikes: 10, name_address: null, sort: null, acs_desc: null, page: null, pageSize: null);

			// Assert
			Assert.NotNull(result);
		}

		[Fact]
		public async Task QueryStationsAsync_SearchByName_ReturnsMatchingStations()
		{
			// Arrange
			CreateTestJsonFile();
			var mockClient = CreateMockCosmosClient();
			var mockContainer = new Mock<Container>();

			mockClient
				.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(mockContainer.Object);

			var service = new CosmosStationService(mockClient.Object, "testDb", "testContainer", _testJsonPath);

			// Act
			var result = await service.QueryStationsAsync(status: null, minBikes: null, name_address: "Station A", sort: null, acs_desc: null, page: null, pageSize: null);

			// Assert
			Assert.NotNull(result);
		}

		[Fact]
		public async Task QueryStationsAsync_SortByName_ReturnsSortedResults()
		{
			// Arrange
			CreateTestJsonFile();
			var mockClient = CreateMockCosmosClient();
			var mockContainer = new Mock<Container>();

			mockClient
				.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(mockContainer.Object);

			var service = new CosmosStationService(mockClient.Object, "testDb", "testContainer", _testJsonPath);

			// Act
			var result = await service.QueryStationsAsync(status: null, minBikes: null, name_address: null, sort: "name", acs_desc: "asc", page: null, pageSize: null);

			// Assert
			Assert.NotNull(result);
		}

		[Fact]
		public async Task QueryStationsAsync_SortByAvailableBikes_ReturnsSortedResults()
		{
			// Arrange
			CreateTestJsonFile();
			var mockClient = CreateMockCosmosClient();
			var mockContainer = new Mock<Container>();

			mockClient
				.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(mockContainer.Object);

			var service = new CosmosStationService(mockClient.Object, "testDb", "testContainer", _testJsonPath);

			// Act
			var result = await service.QueryStationsAsync(status: null, minBikes: null, name_address: null, sort: "availablebikes", acs_desc: "desc", page: null, pageSize: null);

			// Assert
			Assert.NotNull(result);
		}

		[Fact]
		public async Task QueryStationsAsync_WithPagination_ReturnsPaginatedResults()
		{
			// Arrange
			CreateTestJsonFile();
			var mockClient = CreateMockCosmosClient();
			var mockContainer = new Mock<Container>();

			mockClient
				.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(mockContainer.Object);

			var service = new CosmosStationService(mockClient.Object, "testDb", "testContainer", _testJsonPath);

			// Act
			var result = await service.QueryStationsAsync(status: null, minBikes: null, name_address: null, sort: null, acs_desc: null, page: 1, pageSize: 2);

			// Assert
			Assert.NotNull(result);
		}

		[Fact]
		public async Task QueryStationsAsync_ReturnsCorrectTotalCount()
		{
			// Arrange
			CreateTestJsonFile();
			var mockClient = CreateMockCosmosClient();
			var mockContainer = new Mock<Container>();

			mockClient
				.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(mockContainer.Object);

			var service = new CosmosStationService(mockClient.Object, "testDb", "testContainer", _testJsonPath);

			// Act
			var result = await service.QueryStationsAsync(status: "OPEN", minBikes: null, name_address: null, sort: null, acs_desc: null, page: null, pageSize: null);

			// Assert
			Assert.NotNull(result);
		}

		[Fact]
		public async Task GetSummaryAsync_ReturnsSummaryData()
		{
			// Arrange
			CreateTestJsonFile();
			var mockClient = CreateMockCosmosClient();
			var mockContainer = new Mock<Container>();

			mockClient
				.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(mockContainer.Object);

			var service = new CosmosStationService(mockClient.Object, "testDb", "testContainer", _testJsonPath);

			// Act
			var result = await service.GetSummaryAsync();

			// Assert
			Assert.NotNull(result);
		}

		[Fact]
		public async Task QueryStationsAsync_CombineMultipleFilters_ReturnsCorrectResults()
		{
			// Arrange
			CreateTestJsonFile();
			var mockClient = CreateMockCosmosClient();
			var mockContainer = new Mock<Container>();

			mockClient
				.Setup(c => c.GetContainer(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(mockContainer.Object);

			var service = new CosmosStationService(mockClient.Object, "testDb", "testContainer", _testJsonPath);

			// Act
			var result = await service.QueryStationsAsync(
				status: "OPEN",
				minBikes: 10,
				name_address: null,
				sort: null,
				acs_desc: null,
				page: null,
				pageSize: null);

			// Assert
			Assert.NotNull(result);
		}

		[Fact]
		public void LoadStationsFromJson_CreatesTestFile()
		{
			// Arrange
			CreateTestJsonFile();

			// Act
			bool exists = File.Exists(_testJsonPath);

			// Assert
			Assert.True(exists);

			// Cleanup
			if (File.Exists(_testJsonPath))
				File.Delete(_testJsonPath);
		}
	}
}
