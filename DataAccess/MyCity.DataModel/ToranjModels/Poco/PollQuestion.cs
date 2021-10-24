using System.Collections.Generic;

namespace MyCity.DataModel.ToranjModels {
	public class PollQuestion {
		public int ID { get; set; }
		public int PollID { get; set; }
		public string Title { get; set; }
		public int SelectedCount { get; set; }
		public int Ordering { get; set; }

		public virtual Poll Poll { get; set; }
		public virtual ICollection<PollQuestionAnswer> PollQuestionAnswers { get; set; }
	}
}
