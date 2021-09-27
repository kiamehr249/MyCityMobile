using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCity.DataModel.ToranjModels
{
    public class News
    {
        public int ID { get; set; }
        public int AgencyID { get; set; }
        public string Title { get; set; }
        public string LeadText { get; set; }
        public string FullText { get; set; }
        public string FPicture { get; set; }
        public string DatePublished { get; set; }
        public string TimePublished { get; set; }
        public string DateShown { get; set; }
        public string TimeShown { get; set; }
        public bool Enabled { get; set; }
        public int Hit { get; set; }
        public string SPicture { get; set; }
        public string DateEndShown { get; set; }
        public string TimeEndShown { get; set; }
        public int PortalID { get; set; }
        public int Ordering { get; set; }
        public bool IsDeleted { get; set; }
        public string ThumbnailSmall { get; set; }
        public string ThumbnailMid { get; set; }

        public virtual NewsAgency NewsAgency { get; set; }
    }
}
