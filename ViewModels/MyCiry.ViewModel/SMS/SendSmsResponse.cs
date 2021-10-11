using System.Collections.Generic;

namespace MyCiry.ViewModel.SMS {
	public class SendSmsResponse: BaseSmsResponse {
		public List<SmsId> Ids { get; set; }
		public string BatchKey { get; set; }
	}
}
