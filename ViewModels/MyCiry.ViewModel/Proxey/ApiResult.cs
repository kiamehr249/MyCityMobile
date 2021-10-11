namespace MyCiry.ViewModel.Proxey {
	public class ApiResult<T> {
		public int Status { get; set; }
		public T Content { get; set; }
		public string StrResult { get; set; }
	}
}
