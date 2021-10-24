using MyCity.DataAccess;

namespace MyCity.DataModel.ToranjModels {
	public interface IPollCategoryService : IDataService<PollCategory> {
	}

	public class PollCategoryService : DataService<PollCategory>, IPollCategoryService {
		public PollCategoryService(IToranjUnitOfWork uow) : base(uow) {
		}
	}
}