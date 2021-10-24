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
        public DbSet<News> News { get; set; }
        public DbSet<Gallery> Galleries { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Media> Medias { get; set; }

		public DbSet<PollCategory> PollCategories { get; set; }
		public DbSet<Poll> Polls { get; set; }
		public DbSet<PollQuestion> PollQuestions { get; set; }
		public DbSet<PollQuestionAnswer> PollQuestionAnswers { get; set; }


		protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new NewsAgencyMap());
            builder.ApplyConfiguration(new NewsMap());
            builder.ApplyConfiguration(new GalleryMap());
            builder.ApplyConfiguration(new AlbumMap());
            builder.ApplyConfiguration(new MediaMap());

			builder.ApplyConfiguration(new PollCategoryMap());
			builder.ApplyConfiguration(new PollMap());
			builder.ApplyConfiguration(new PollQuestionMap());
			builder.ApplyConfiguration(new PollQuestionAnswerMap());
		}
    }
}
