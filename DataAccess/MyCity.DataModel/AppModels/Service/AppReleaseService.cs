using MyCity.DataAccess;

namespace MyCity.DataModel.AppModels {
	public interface IAppReleaseService : IDataService<AppRelease> {
	}

	public class AppReleaseService : DataService<AppRelease>, IAppReleaseService {
		public AppReleaseService(IMyUnitOfWork uow) : base(uow) {
		}
	}
}