namespace fs_2025_assessment_1_73599_blazorapp.Models
{
	public class Station
	{
		public int number { get; set; }
		public string id => number.ToString();
		public string contract_name { get; set; }
		public string name { get; set; }
		public string address { get; set; }
		public Position position { get; set; }
		public bool banking { get; set; }
		public bool bonus { get; set; }
		public int bike_stands { get; set; }
		public int available_bike_stands { get; set; }
		public int available_bikes { get; set; }
		public string status { get; set; }
		public long last_update { get; set; }

		// UTC conversion
		public DateTimeOffset last_update_datetime =>
			DateTimeOffset.FromUnixTimeMilliseconds(last_update);

		// Europe/Dublin conversion
		public DateTimeOffset last_update_local =>
			TimeZoneInfo.ConvertTime(last_update_datetime,
				TimeZoneInfo.FindSystemTimeZoneById("Europe/Dublin"));

		// Calculated occupancy percentage
		public int OccupancyPercentage =>
			bike_stands > 0 ? (int)((available_bikes * 100) / bike_stands) : 0;
	}

	public class Position
	{
		public float lat { get; set; }
		public float lng { get; set; }
	}
}