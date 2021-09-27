using System.Collections.Generic;

namespace MyCity.DataModel.ToranjModels
{
    public class Album
    {
        public int ID { get; set; }
        public int GalleryID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public string PhotographerName { get; set; }
        public int Ordering { get; set; }
        public int Hit { get; set; }
        public string Logo { get; set; }
        public bool IsReport { get; set; }
        public string DateOfPhotos { get; set; }
        public bool IsDeleted { get; set; }

        public virtual Gallery Gallery { get; set; }
        public virtual ICollection<Media> Medias { get; set; }
    }
}
