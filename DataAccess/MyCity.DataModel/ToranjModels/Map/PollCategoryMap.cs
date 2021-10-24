using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.ToranjModels {
	public class PollCategoryMap : IEntityTypeConfiguration<PollCategory> {
		public void Configure(EntityTypeBuilder<PollCategory> builder) {
			builder.HasKey(x => x.ID);
			builder.ToTable("PollCategories");
		}
	}
}