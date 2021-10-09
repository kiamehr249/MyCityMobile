using Microsoft.Extensions.Configuration;

namespace MyCity.DataModel.ToranjModels
{
    public interface IToranjServices
    {
        INewsAgencyService iNewsAgencyServ { get; set; }
        INewsService iNewsServ { get; set; }
        IGalleryService iGalleryServ { get; set; }
        IAlbumService iAlbumServ { get; set; }
        IMediaService iMediaServ { get; set; }
    }

    public class ToranjServices : IToranjServices
    {
        public INewsAgencyService iNewsAgencyServ { get; set; }
        public INewsService iNewsServ { get; set; }
        public IGalleryService iGalleryServ { get; set; }
        public IAlbumService iAlbumServ { get; set; }
        public IMediaService iMediaServ { get; set; }

        public ToranjServices(IConfiguration config)
        {
            IToranjUnitOfWork uow = new ToranjDbContext(config.GetConnectionString("ToranjDb"));
            iNewsAgencyServ = new NewsAgencyService(uow);
            iNewsServ = new NewsService(uow);
            iGalleryServ = new GalleryService(uow);
            iAlbumServ = new AlbumService(uow);
            iMediaServ = new MediaService(uow);
        }
    }
}
