using Xunit;
using fs_2025_assessment_1_73599.Models;

namespace fs_2025_assessment_1_73599.Tests
{
	public class StationTests
	{
		[Fact]
		public void Station_OccupancyPercentage_CalculatesCorrectly()
		{
			// Arrange
			var station = new Station
			{
				number = 1,
				name = "Test Station",
				available_bikes = 10,
				bike_stands = 20
			};

			// Act
			int occupancy = station.OccupancyPercentage;

			// Assert
			Assert.Equal(50, occupancy);
		}

		[Fact]
		public void Station_OccupancyPercentage_HandlesZeroStands()
		{
			// Arrange
			var station = new Station
			{
				number = 1,
				name = "Test Station",
				available_bikes = 10,
				bike_stands = 0
			};

			// Act
			int occupancy = station.OccupancyPercentage;

			// Assert
			Assert.Equal(0, occupancy);
		}

		[Fact]
		public void Station_Properties_SetCorrectly()
		{
			// Arrange & Act
			var station = new Station
			{
				number = 42,
				name = "Smithfield",
				address = "Dublin",
				status = "OPEN"
			};

			// Assert
			Assert.Equal(42, station.number);
			Assert.Equal("Smithfield", station.name);
			Assert.Equal("Dublin", station.address);
			Assert.Equal("OPEN", station.status);
		}
	}
}
