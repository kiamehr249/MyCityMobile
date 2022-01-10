using MyCiry.ViewModel;
using System;
using System.Collections.Generic;

namespace MyCity.DataModel.AppModels {
	public class Report {
		public int Id { get; set; }
		public int TargetAreaId { get; set; }
		public string Subject { get; set; }
		public string UserText { get; set; }
		public string FileSrc { get; set; }
		public FileType? FileType { get; set; }
		public double Latlatitude { get; set; }
		public double Longitude { get; set; }
		public ReportStatus Status { get; set; }
		public int UserId { get; set; }
		public int? ExpertId { get; set; }
		public int? InspectorId { get; set; }
		public int ComplaintCount { get; set; }
		public DateTime CraeteDate { get; set; }
		public DateTime? ModifyDate { get; set; }

		public virtual TargetArea TargetArea { get; set; }
		public virtual AppUser User { get; set; }
		public virtual AppUser Expert { get; set; }
		public virtual ICollection<ReportExpert> ReportExperts { get; set; }
	}
}
