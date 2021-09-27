namespace MyCity.DataModel.ToranjModels
{
    //MediaType I image V video 
    public class Media
    {
        public int ID { get; set; }
        public int AlbumID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public int Ordering { get; set; }
        //public byte PhotoByte { get; set; }
        public string PhotoType { get; set; }
        //public bool Selected { get; set; }
        //public bool SelectedGallery { get; set; }
        //public bool SaveInDB { get; set; }
        public string FileName { get; set; }
        public string Thumbnail { get; set; }
        public string Thumbnail2 { get; set; }
        public string BackgroundPlayer { get; set; }
        public int Hit { get; set; }
        public string MediaType { get; set; }
        public string BackgroundPlayer2 { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual Album Album { get; set; }
    }
}
