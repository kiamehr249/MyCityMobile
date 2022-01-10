using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.AppModels {
	public class ReportExpertMap : IEntityTypeConfiguration<ReportExpert> {
		public void Configure(EntityTypeBuilder<ReportExpert> builder) {
			builder.HasKey(x => x.Id);
			builder.ToTable("ReportExperts");

			builder.HasOne(x => x.Expert)
				.WithMany(x => x.ReportExperts)
				.HasForeignKey(x => x.ExpertId);

			builder.HasOne(x => x.Report)
				.WithMany(x => x.ReportExperts)
				.HasForeignKey(x => x.ReportId);
		}
	}
}