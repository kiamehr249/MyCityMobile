using System.Collections.Generic;

namespace MyCity.DataModel.ToranjModels {
	public class Poll {
		public int ID { get; set; }
		public string Title { get; set; }
		public bool Enabled { get; set; }
		public int PortalID { get; set; }
		public string TextQuestion { get; set; }
		public bool ShowTextQuestion { get; set; }
		public bool ViewResult { get; set; }
		public int? CategoryID { get; set; }

		public virtual PollCategory PollCategory { get; set; }
		public virtual ICollection<PollQuestion> PollQuestions { get; set; }
		public virtual ICollection<PollQuestionAnswer> PollQuestionAnswers { get; set; }
	}
}
