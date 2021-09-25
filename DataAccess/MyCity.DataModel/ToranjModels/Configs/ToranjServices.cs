using Microsoft.Extensions.Configuration;

namespace MyCity.DataModel.ToranjModels.Configs
{
    public interface IToranjServices
    {
        INewsAgencyService iNewsAgencyServ { get; set; }
    }

    public class ToranjServices : IToranjServices
    {
        public INewsAgencyService iNewsAgencyServ { get; set; }


        public ToranjServices(IConfiguration Configuration)
        {
            IToranjUnitOfWork uow = new ToranjDbContext(Configuration.GetConnectionString("SystemBase"));
            iNewsAgencyServ = new NewsAgencyService(uow);
        }
    }
}
