using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.AppModels {
	public class RatingSettingMap : IEntityTypeConfiguration<RatingSetting> {
		public void Configure(EntityTypeBuilder<RatingSetting> builder) {
			builder.HasKey(x => x.Id);
			builder.ToTable("RatingSettings");
		}
	}
}