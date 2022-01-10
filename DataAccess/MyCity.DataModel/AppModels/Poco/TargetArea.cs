using System;
using System.Collections.Generic;

namespace MyCity.DataModel.AppModels {
	public class TargetArea {
		public int Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public double Latlatitude { get; set; }
		public double Longitude { get; set; }
		public int CreateBy { get; set; }
		public DateTime CraeteDate { get; set; }
		public int? ModifyBy { get; set; }
		public DateTime? ModifyDate { get; set; }

		public virtual ICollection<Report> Reports { get; set; }
		public virtual ICollection<UserProfile> UserProfiles { get; set; }
	}
}
