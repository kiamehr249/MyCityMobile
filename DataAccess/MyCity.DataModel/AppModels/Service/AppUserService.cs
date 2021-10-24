using MyCity.DataAccess;

namespace MyCity.DataModel.AppModels {
	public interface IAppUserService : IDataService<AppUser> {
	}

	public class AppUserService : DataService<AppUser>, IAppUserService {
		public AppUserService(IMyUnitOfWork uow) : base(uow) {
		}
	}
}