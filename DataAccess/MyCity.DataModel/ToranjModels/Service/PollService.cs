using MyCity.DataAccess;

namespace MyCity.DataModel.ToranjModels {
	public interface IPollService : IDataService<Poll> {
	}

	public class PollService : DataService<Poll>, IPollService {
		public PollService(IToranjUnitOfWork uow) : base(uow) {
		}
	}
}