using MyCiry.ViewModel;
using System;

namespace MyCity.DataModel.AppModels {
	public class ReportExpert {
		public int Id { get; set; }
		public int ExpertId { get; set; }
		public int ReportId { get; set; }
		public string Description { get; set; }
		public ExpertAction Action { get; set; }
		public DateTime CraeteDate { get; set; }
		public DateTime? ModifyDate { get; set; }

		public virtual AppUser Expert { get; set; }
		public virtual Report Report { get; set; }
	}
}
