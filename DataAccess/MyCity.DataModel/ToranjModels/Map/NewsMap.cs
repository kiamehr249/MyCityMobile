using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.ToranjModels
{
    public class NewsMap : IEntityTypeConfiguration<News>
    {
        public void Configure(EntityTypeBuilder<News> builder)
        {
            builder.HasKey(x => x.ID);
            builder.ToTable("News");

            builder.HasOne(x => x.NewsAgency)
                .WithMany(x => x.News)
                .HasForeignKey(x => x.AgencyID).IsRequired(false);
        }
    }
}