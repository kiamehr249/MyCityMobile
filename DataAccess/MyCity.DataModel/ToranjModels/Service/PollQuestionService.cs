using MyCity.DataAccess;

namespace MyCity.DataModel.ToranjModels {
	public interface IPollQuestionService : IDataService<PollQuestion> {
	}

	public class PollQuestionService : DataService<PollQuestion>, IPollQuestionService {
		public PollQuestionService(IToranjUnitOfWork uow) : base(uow) {
		}
	}
}