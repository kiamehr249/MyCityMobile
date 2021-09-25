using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.ToranjModels
{
    public class NewsAgencyMap : IEntityTypeConfiguration<NewsAgency>
    {
        public void Configure(EntityTypeBuilder<NewsAgency> builder)
        {
            builder.HasKey(x => x.ID);
            builder.ToTable("NewsAgencies");

            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Child)
                .HasForeignKey(x => x.ParentID).IsRequired(false);
        }
    }
}