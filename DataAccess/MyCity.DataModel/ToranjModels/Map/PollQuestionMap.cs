using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.ToranjModels {
	public class PollQuestionMap : IEntityTypeConfiguration<PollQuestion> {
		public void Configure(EntityTypeBuilder<PollQuestion> builder) {
			builder.HasKey(x => x.ID);
			builder.ToTable("PollQuestions");

			builder.HasOne(x => x.Poll)
				.WithMany(x => x.PollQuestions)
				.HasForeignKey(x => x.PollID);
		}
	}
}