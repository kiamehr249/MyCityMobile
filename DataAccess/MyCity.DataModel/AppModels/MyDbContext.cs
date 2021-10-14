using Microsoft.EntityFrameworkCore;
using MyCity.DataAccess;

namespace MyCity.DataModel.AppModels {
	public class MyDbContext : RootDbContext, IMyUnitOfWork {

		public MyDbContext(string connectionString) : base(connectionString) {

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseLazyLoadingProxies();
		}

		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<AppRelease> AppReleases { get; set; }

		protected override void OnModelCreating(ModelBuilder builder) {
			builder.ApplyConfiguration(new UserProfileMap());
			builder.ApplyConfiguration(new AppReleaseMap());
		}
	}
}
