using Microsoft.AspNetCore.Http;

namespace MyCiry.ViewModel {
    public class SaveFileRequest
    {
        public IFormFile File { get; set; }
        public string RootPath { get; set; }
        public string UnitPath { get; set; }
    }
}
