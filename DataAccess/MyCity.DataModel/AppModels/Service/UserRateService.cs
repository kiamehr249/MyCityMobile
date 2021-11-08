using MyCity.DataAccess;

namespace MyCity.DataModel.AppModels {
	public interface IUserRateService : IDataService<UserRate> {
	}

	public class UserRateService : DataService<UserRate>, IUserRateService {
		public UserRateService(IMyUnitOfWork uow) : base(uow) {
		}
	}
}