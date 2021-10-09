using System.Collections.Generic;

namespace MyCiry.ViewModel.SMS {
	public class SendSmsApiRequest {
		public List<string> Messages { get; set; }
		public List<string> MobileNumbers { get; set; }
		public string LineNumber { get; set; }
		public string SendDateTime { get; set; }
		public string CanContinueInCaseOfError { get; set; }
	}
}
