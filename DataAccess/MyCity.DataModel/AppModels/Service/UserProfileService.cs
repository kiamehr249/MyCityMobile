using MyCity.DataAccess;

namespace MyCity.DataModel.AppModels {
	public interface IUserProfileService : IDataService<UserProfile> {
	}


	public class UserProfileService : DataService<UserProfile>, IUserProfileService {
		public UserProfileService(IMyUnitOfWork uow) : base(uow) {
		}
	}
}