using Microsoft.Extensions.Configuration;

namespace MyCity.DataModel.ToranjModels
{
    public interface IToranjServices
    {
        INewsAgencyService iNewsAgencyServ { get; set; }
    }

    public class ToranjServices : IToranjServices
    {
        public INewsAgencyService iNewsAgencyServ { get; set; }


        public ToranjServices(IConfiguration config)
        {
            IToranjUnitOfWork uow = new ToranjDbContext(config.GetConnectionString("ToranjDb"));
            iNewsAgencyServ = new NewsAgencyService(uow);
        }
    }
}
