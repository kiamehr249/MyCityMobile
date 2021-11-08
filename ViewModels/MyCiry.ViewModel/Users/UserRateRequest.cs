namespace MyCiry.ViewModel.Users {
	public class UserRateRequest {
		public int RateType { get; set; }
		public int? ReferenceId { get; set; }
		public string ReferenceTitle { get; set; }
		public double Duration { get; set; }
	}
}
