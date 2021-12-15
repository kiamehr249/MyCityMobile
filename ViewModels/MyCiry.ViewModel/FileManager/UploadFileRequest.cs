using Microsoft.AspNetCore.Http;

namespace MyCiry.ViewModel {
	public class UploadFileRequest
    {
        public IFormFile File { get; set; }
        public string ContentId { get; set; }
        public string ContentKey { get; set; }
        public string RootKey { get; set; }
    }
}
