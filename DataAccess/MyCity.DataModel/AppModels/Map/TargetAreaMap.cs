using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.AppModels {
	public class TargetAreaMap : IEntityTypeConfiguration<TargetArea> {
		public void Configure(EntityTypeBuilder<TargetArea> builder) {
			builder.HasKey(x => x.Id);
			builder.ToTable("TargetAreas");
		}
	}
}