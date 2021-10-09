using Microsoft.EntityFrameworkCore;
using MyCity.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCity.DataModel.AppModels {
	public class MyDbContext : RootDbContext, IMyUnitOfWork {

		public MyDbContext(string connectionString) : base(connectionString) {

		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
			optionsBuilder.UseLazyLoadingProxies();
		}

		public DbSet<UserProfile> UserProfiles { get; set; }


		protected override void OnModelCreating(ModelBuilder builder) {
			builder.ApplyConfiguration(new UserProfileMap());
		}
	}
}
