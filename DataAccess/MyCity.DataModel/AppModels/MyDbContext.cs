using Microsoft.EntityFrameworkCore;
using MyCity.DataAccess;

namespace MyCity.DataModel.AppModels {
	public class MyDbContext : RootDbContext, IMyUnitOfWork {

		public MyDbContext(string connectionString) : base(connectionString) {

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseLazyLoadingProxies();
		}

		public DbSet<AppUser> Users { get; set; }

		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<AppRelease> AppReleases { get; set; }
		public DbSet<RatingSetting> RatingSettings { get; set; }
		public DbSet<UserRate> UserRates { get; set; }
		public DbSet<TargetArea> TargetAreas { get; set; }
		public DbSet<Report> Reports { get; set; }
		public DbSet<ReportExpert> ReportExperts { get; set; }


		protected override void OnModelCreating(ModelBuilder builder) {
			builder.ApplyConfiguration(new AppUserMap());
			builder.ApplyConfiguration(new UserProfileMap());
			builder.ApplyConfiguration(new AppReleaseMap());
			builder.ApplyConfiguration(new RatingSettingMap());
			builder.ApplyConfiguration(new UserRateMap());
			builder.ApplyConfiguration(new TargetAreaMap());
			builder.ApplyConfiguration(new ReportMap());
			builder.ApplyConfiguration(new ReportExpertMap());
		}
	}
}
