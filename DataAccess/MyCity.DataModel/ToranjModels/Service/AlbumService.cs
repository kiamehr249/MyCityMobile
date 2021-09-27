using MyCity.DataAccess;

namespace MyCity.DataModel.ToranjModels
{
    public interface IAlbumService : IDataService<Album>
    {
    }

    public class AlbumService : DataService<Album>, IAlbumService
    {
        public AlbumService(IToranjUnitOfWork uow) : base(uow)
        {
        }
    }
}