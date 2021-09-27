using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.ToranjModels
{
    public class MediaMap : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.HasKey(x => x.ID);
            builder.ToTable("ImagePhotos");

            builder.HasOne(x => x.Album)
                .WithMany(x => x.Medias)
                .HasForeignKey(x => x.AlbumID);
        }
    }
}