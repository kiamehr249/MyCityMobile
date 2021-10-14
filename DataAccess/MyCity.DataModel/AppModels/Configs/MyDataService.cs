using Microsoft.Extensions.Configuration;

namespace MyCity.DataModel.AppModels {
	public interface IMyDataService {
		IUserProfileService iUserProfileServ { get; set; }
		IAppReleaseService iAppReleaseServ { get; set; }
	}

	public class MyDataService : IMyDataService {
		IConfiguration config;
		public IUserProfileService iUserProfileServ { get; set; }
		public IAppReleaseService iAppReleaseServ { get; set; }

		public MyDataService(IConfiguration config) {
			IMyUnitOfWork uow = new MyDbContext(config.GetConnectionString("SystemBase"));
			iUserProfileServ = new UserProfileService(uow);
			iAppReleaseServ = new AppReleaseService(uow);
		}



	}
}
