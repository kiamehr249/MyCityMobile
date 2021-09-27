using MyCity.DataAccess;

namespace MyCity.DataModel.ToranjModels
{
    public interface IGalleryService : IDataService<Gallery>
    {

    }

    public class GalleryService : DataService<Gallery>, IGalleryService
    {
        public GalleryService(IToranjUnitOfWork uow) : base(uow)
        {
        }
    }
}