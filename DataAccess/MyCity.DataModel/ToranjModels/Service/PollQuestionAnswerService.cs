using MyCity.DataAccess;

namespace MyCity.DataModel.ToranjModels {
	public interface IPollQuestionAnswerService : IDataService<PollQuestionAnswer> {
	}

	public class PollQuestionAnswerService : DataService<PollQuestionAnswer>, IPollQuestionAnswerService {
		public PollQuestionAnswerService(IToranjUnitOfWork uow) : base(uow) {
		}
	}
}