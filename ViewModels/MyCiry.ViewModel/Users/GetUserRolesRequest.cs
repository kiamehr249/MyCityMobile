namespace MyCiry.ViewModel.Users {
	public class GetUserRolesRequest : Pager {
		public int UserId { get; set; }
		public string RoleName { get; set; }
	}
}
