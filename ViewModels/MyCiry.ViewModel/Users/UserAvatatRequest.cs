using Microsoft.AspNetCore.Http;

namespace MyCiry.ViewModel.Users {
	public class UserAvatatRequest {
		public IFormFile Avatar { get; set; }
	}
}
