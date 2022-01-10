using Microsoft.AspNetCore.Http;

namespace MyCiry.ViewModel {
	public class ReportFileRequest {
		public int Id { get; set; }
		public int FileType { get; set; }
		public IFormFile File { get; set; }
	}
}
