using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyCity.DataModel.ToranjModels {
	public class PollQuestionAnswerMap : IEntityTypeConfiguration<PollQuestionAnswer> {
		public void Configure(EntityTypeBuilder<PollQuestionAnswer> builder) {
			builder.HasKey(x => x.ID);
			builder.ToTable("PollQuestionAnswers");

			builder.HasOne(x => x.Poll)
				.WithMany(x => x.PollQuestionAnswers)
				.HasForeignKey(x => x.PollID);

			builder.HasOne(x => x.PollQuestion)
				.WithMany(x => x.PollQuestionAnswers)
				.HasForeignKey(x => x.QuestionID).IsRequired(false);
		}
	}
}