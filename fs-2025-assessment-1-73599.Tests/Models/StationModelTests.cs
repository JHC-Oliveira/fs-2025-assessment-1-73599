using fs_2025_assessment_1_73599.Models;
using Xunit;

namespace fs_2025_assessment_1_73599.Tests.Models
{
	public class StationModelTests
	{
		[Fact]
		public void Station_IdProperty_ReturnsNumberAsString()
		{
			// Arrange
			var station = new Station { number = 42 };

			// Act
			var id = station.id;

			// Assert
			Assert.Equal("42", id);
		}

		[Fact]
		public void Station_LastUpdateDatetimeProperty_ConvertsUnixTimestampToDateTimeOffset()
		{
			// Arrange
			long unixTimestamp = 1609459200000; // 2021-01-01 00:00:00 UTC
			var station = new Station { last_update = unixTimestamp };

			// Act
			var dateTime = station.last_update_datetime;

			// Assert
			Assert.Equal(2021, dateTime.Year);
			Assert.Equal(1, dateTime.Month);
			Assert.Equal(1, dateTime.Day);
		}

		[Fact]
		public void Station_LastUpdateLocalProperty_ConvertsDublinTime()
		{
			// Arrange
			long unixTimestamp = 1609459200000; // 2021-01-01 00:00:00 UTC
			var station = new Station { last_update = unixTimestamp };

			// Act
			var localDateTime = station.last_update_local;

			// Assert
			Assert.NotNull(localDateTime);
			// Dublin is UTC+0 in January
			Assert.Equal(2021, localDateTime.Year);
			Assert.Equal(1, localDateTime.Month);
			Assert.Equal(1, localDateTime.Day);
		}

		[Fact]
		public void Station_PropertiesCanBeSet()
		{
			// Arrange & Act
			var station = new Station
			{
				number = 1,
				name = "Test Station",
				address = "123 Test St",
				available_bikes = 10,
				bike_stands = 20,
				status = "OPEN",
				contract_name = "Dublin",
				banking = true,
				bonus = false,
				available_bike_stands = 10,
				last_update = 1609459200000,
				position = new Position { lat = 53.3498f, lng = -6.2603f }
			};

			// Assert
			Assert.Equal(1, station.number);
			Assert.Equal("Test Station", station.name);
			Assert.Equal("123 Test St", station.address);
			Assert.Equal(10, station.available_bikes);
			Assert.Equal(20, station.bike_stands);
			Assert.Equal("OPEN", station.status);
			Assert.Equal("Dublin", station.contract_name);
			Assert.True(station.banking);
			Assert.False(station.bonus);
			Assert.Equal(53.3498f, station.position.lat);
			Assert.Equal(-6.2603f, station.position.lng);
		}

		[Fact]
		public void Station_WithZeroBikes_PropertiesAreValid()
		{
			// Arrange & Act
			var station = new Station
			{
				number = 1,
				name = "Empty Station",
				available_bikes = 0,
				bike_stands = 20,
				status = "OPEN"
			};

			// Assert
			Assert.Equal(0, station.available_bikes);
			Assert.Equal(20, station.bike_stands);
		}
	}
}
