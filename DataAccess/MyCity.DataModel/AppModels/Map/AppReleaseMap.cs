using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.AppModels {
	public class AppReleaseMap : IEntityTypeConfiguration<AppRelease> {
		public void Configure(EntityTypeBuilder<AppRelease> builder) {
			builder.HasKey(x => x.Id);
			builder.ToTable("AppReleases");
		}
	}
}