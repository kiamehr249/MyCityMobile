using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.AppModels {
	public class AppUserMap : IEntityTypeConfiguration<AppUser> {
		public void Configure(EntityTypeBuilder<AppUser> builder) {
			builder.HasKey(x => x.Id);
			builder.ToTable("Users");
		}
	}
}