using MyCity.DataAccess;

namespace MyCity.DataModel.ToranjModels
{
    public interface IMediaService : IDataService<Media>
    {
    }

    public class MediaService : DataService<Media>, IMediaService
    {
        public MediaService(IToranjUnitOfWork uow) : base(uow)
        {
        }
    }
}