using System;
using System.Collections.Generic;

namespace MyCity.DataModel.ToranjModels
{
    public class NewsAgency
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Ordering { get; set; }
        public int PortalID { get; set; }
        public string IconURI { get; set; }
        public string PictureURI { get; set; }
        public bool Enabled { get; set; }

        public virtual NewsAgency Parent { get; set; }
        public virtual ICollection<NewsAgency> Child { get; set; }
    }
}
