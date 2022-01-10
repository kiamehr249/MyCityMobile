using MyCity.DataAccess;

namespace MyCity.DataModel.AppModels {
	public interface IReportExpertService : IDataService<ReportExpert> {
	}

	public class ReportExpertService : DataService<ReportExpert>, IReportExpertService {
		public ReportExpertService(IMyUnitOfWork uow) : base(uow) {
		}
	}
}