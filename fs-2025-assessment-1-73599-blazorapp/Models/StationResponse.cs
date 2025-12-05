namespace fs_2025_assessment_1_73599_blazorapp.Models
{
	public class StationResponse
	{
		public List<Station> data { get; set; } = new();
		public int total { get; set; }
		public int page { get; set; }
		public int pageSize { get; set; }
	}

	public class SummaryResponse
	{
		public int total_stations { get; set; }
		public int open_stations { get; set; }
		public int closed_stations { get; set; }
		public int total_bikes { get; set; }
		public int total_stands { get; set; }
	}
}