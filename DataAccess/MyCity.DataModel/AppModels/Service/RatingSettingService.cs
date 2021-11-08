using MyCity.DataAccess;

namespace MyCity.DataModel.AppModels {
	public interface IRatingSettingService : IDataService<RatingSetting> {
	}

	public class RatingSettingService : DataService<RatingSetting>, IRatingSettingService {
		public RatingSettingService(IMyUnitOfWork uow) : base(uow) {
		}
	}
}