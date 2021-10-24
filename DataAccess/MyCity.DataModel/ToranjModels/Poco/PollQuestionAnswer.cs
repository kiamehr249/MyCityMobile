namespace MyCity.DataModel.ToranjModels {
	public class PollQuestionAnswer {
		public int ID { get; set; }
		public int PollID { get; set; }
		public string UserAnswer { get; set; }
		public string UserIP { get; set; }
		public int? QuestionID { get; set; }

		public virtual Poll Poll { get; set; }
		public virtual PollQuestion PollQuestion { get; set; }
	}
}
