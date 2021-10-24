using Microsoft.Extensions.Configuration;

namespace MyCity.DataModel.AppModels {
	public interface IMyDataService {
		IAppUserService iAppUserServ { get; set; }
		IUserProfileService iUserProfileServ { get; set; }
		IAppReleaseService iAppReleaseServ { get; set; }
	}

	public class MyDataService : IMyDataService {
		IConfiguration config;
		public IAppUserService iAppUserServ { get; set; }
		public IUserProfileService iUserProfileServ { get; set; }
		public IAppReleaseService iAppReleaseServ { get; set; }

		public MyDataService(IConfiguration config) {
			IMyUnitOfWork uow = new MyDbContext(config.GetConnectionString("SystemBase"));
			iAppUserServ = new AppUserService(uow);
			iUserProfileServ = new UserProfileService(uow);
			iAppReleaseServ = new AppReleaseService(uow);
		}



	}
}
