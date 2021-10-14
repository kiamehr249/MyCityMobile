using System;

namespace MyCity.DataModel.AppModels {
	public class AppRelease {
		public int Id { get; set; }
		public string VersionName { get; set; }
		public string VersionNo { get; set; }
		public string Url { get; set; }
		public DateTime CreateDate { get; set; }
	}
}
