using MyCity.DataAccess;

namespace MyCity.DataModel.ToranjModels
{
    public interface INewsService : IDataService<News>
    {
    }

    public class NewsService : DataService<News>, INewsService
    {
        public NewsService(IToranjUnitOfWork uow) : base(uow)
        {
        }
    }
}