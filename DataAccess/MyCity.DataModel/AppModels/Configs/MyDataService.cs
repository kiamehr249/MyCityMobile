using Microsoft.Extensions.Configuration;

namespace MyCity.DataModel.AppModels {
	public interface IMyDataService {
		IAppUserService iAppUserServ { get; set; }
		IUserProfileService iUserProfileServ { get; set; }
		IAppReleaseService iAppReleaseServ { get; set; }
		IRatingSettingService iRatingSettingServ { get; set; }
		IUserRateService iUserRateServ { get; set; }
		ITargetAreaService iTargetAreaServ { get; set; }
		IReportService iReportServ { get; set; }
		IReportExpertService iReportExpertServ { get; set; }
	}

	public class MyDataService : IMyDataService {
		public IAppUserService iAppUserServ { get; set; }
		public IUserProfileService iUserProfileServ { get; set; }
		public IAppReleaseService iAppReleaseServ { get; set; }
		public IRatingSettingService iRatingSettingServ { get; set; }
		public IUserRateService iUserRateServ { get; set; }
		public ITargetAreaService iTargetAreaServ { get; set; }
		public IReportService iReportServ { get; set; }
		public IReportExpertService iReportExpertServ { get; set; }

		public MyDataService(IConfiguration config) {
			IMyUnitOfWork uow = new MyDbContext(config.GetConnectionString("SystemBase"));
			iAppUserServ = new AppUserService(uow);
			iUserProfileServ = new UserProfileService(uow);
			iAppReleaseServ = new AppReleaseService(uow);
			iRatingSettingServ = new RatingSettingService(uow);
			iUserRateServ = new UserRateService(uow);
			iTargetAreaServ = new TargetAreaService(uow);
			iReportServ = new ReportService(uow);
			iReportExpertServ = new ReportExpertService(uow);
		}



	}
}
