namespace MyCiry.ViewModel.Users {
	public class RatingSettingRequest {
		public int Id { get; set; }
		public int RateType { get; set; }
		public float Coefficient { get; set; }
		public int PerCount { get; set; }
	}
}
