using System.Collections.Generic;

namespace MyCity.DataModel.ToranjModels {
	public class PollCategory {
		public int ID { get; set; }
		public string Title { get; set; }
		public string ImgUrl { get; set; }
		public bool Enabled { get; set; }
		public int PortalID { get; set; }

		public virtual ICollection<Poll> Polls { get; set; }
	}
}
