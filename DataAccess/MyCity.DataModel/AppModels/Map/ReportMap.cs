using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.AppModels {
	public class ReportMap : IEntityTypeConfiguration<Report> {
		public void Configure(EntityTypeBuilder<Report> builder) {
			builder.HasKey(x => x.Id);
			builder.ToTable("Reports");

			builder.HasOne(x => x.User)
				.WithMany(x => x.Reports)
				.HasForeignKey(x => x.UserId);

			builder.HasOne(x => x.Expert)
				.WithMany(x => x.RepExperts)
				.HasForeignKey(x => x.ExpertId).IsRequired(false);

			builder.HasOne(x => x.TargetArea)
				.WithMany(x => x.Reports)
				.HasForeignKey(x => x.TargetAreaId);
		}
	}
}