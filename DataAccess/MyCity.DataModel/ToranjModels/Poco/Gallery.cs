using System.Collections.Generic;

namespace MyCity.DataModel.ToranjModels
{
    //Types 1 video 2 sound 3 image
    public class Gallery
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int PortalID { get; set; }
        public bool Enabled { get; set; }
        public int GalleryType { get; set; }
        public int Ordering { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Album> Albums { get; set; }

    }
}
