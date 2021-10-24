namespace MyCiry.ViewModel.Users {
	public class GetUserRequest : Pager {
		public int? AccountType { get; set; }
		public bool? Activated { get; set; }
		public int? ExpertStatus { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
	}
}
