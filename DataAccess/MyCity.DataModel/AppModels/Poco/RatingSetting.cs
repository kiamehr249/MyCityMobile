namespace MyCity.DataModel.AppModels {
	public class RatingSetting {
		public int Id { get; set; }
		public RateType RateType { get; set; }
		public double Coefficient { get; set; }
		public int PerCount { get; set; }
	}
}
