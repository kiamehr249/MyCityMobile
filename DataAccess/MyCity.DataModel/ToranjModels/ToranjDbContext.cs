using Microsoft.EntityFrameworkCore;
using MyCity.DataAccess;

namespace MyCity.DataModel.ToranjModels
{
    public class ToranjDbContext : RootDbContext, IToranjUnitOfWork
    {

        public ToranjDbContext(string connectionString) : base(connectionString)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        public DbSet<NewsAgency> NewsAgencies { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new NewsAgencyMap());
        }
    }
}
