using MyCity.DataAccess;

namespace MyCity.DataModel.AppModels {
	public interface IReportService : IDataService<Report> {
	}

	public class ReportService : DataService<Report>, IReportService {
		public ReportService(IMyUnitOfWork uow) : base(uow) {
		}
	}
}