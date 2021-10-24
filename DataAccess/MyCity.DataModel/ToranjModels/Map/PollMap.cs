using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.ToranjModels {
	public class PollMap : IEntityTypeConfiguration<Poll> {
		public void Configure(EntityTypeBuilder<Poll> builder) {
			builder.HasKey(x => x.ID);
			builder.ToTable("Polls");

			builder.HasOne(x => x.PollCategory)
				.WithMany(x => x.Polls)
				.HasForeignKey(x => x.CategoryID).IsRequired(false);
		}
	}
}