namespace MyCiry.ViewModel {
	public class TargetAreaRequest {
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public double Latlatitude { get; set; }
		public double Longitude { get; set; }
		public int CreateBy { get; set; }
		public int? ModifyBy { get; set; }
	}
}
