using MyCity.DataAccess;

namespace MyCity.DataModel.ToranjModels
{
    public interface INewsAgencyService : IDataService<NewsAgency>
    {
    }

    public class NewsAgencyService : DataService<NewsAgency>, INewsAgencyService
    {
        public NewsAgencyService(IToranjUnitOfWork uow) : base(uow)
        {
        }
    }
}