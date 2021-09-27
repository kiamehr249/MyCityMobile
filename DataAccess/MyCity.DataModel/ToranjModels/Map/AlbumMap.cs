using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.ToranjModels
{
    public class AlbumMap : IEntityTypeConfiguration<Album>
    {
        public void Configure(EntityTypeBuilder<Album> builder)
        {
            builder.HasKey(x => x.ID);
            builder.ToTable("ImageAlbums");

            builder.HasOne(x => x.Gallery)
                .WithMany(x => x.Albums)
                .HasForeignKey(x => x.GalleryID).IsRequired(false);
        }
    }
}