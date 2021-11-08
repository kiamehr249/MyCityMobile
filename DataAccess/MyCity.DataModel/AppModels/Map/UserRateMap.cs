using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.AppModels {
	public class UserRateMap : IEntityTypeConfiguration<UserRate> {
		public void Configure(EntityTypeBuilder<UserRate> builder) {
			builder.HasKey(x => x.Id);
			builder.ToTable("UserRates");

			builder.HasOne(x => x.User)
				.WithMany(x => x.UserRates)
				.HasForeignKey(x => x.UserId);
		}
	}
}