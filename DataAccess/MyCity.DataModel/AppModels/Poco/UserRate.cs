using System;

namespace MyCity.DataModel.AppModels {
	public class UserRate {
		public int Id { get; set; }
		public int UserId { get; set; }
		public RateType RateType { get; set; }
		public double RateAmount { get; set; }
		public int? ReferenceId { get; set; }
		public string ReferenceTitle { get; set; }
		public double Duration { get; set; }
		public DateTime CraeteDate { get; set; }

		public virtual AppUser User { get; set; }
	}
}
