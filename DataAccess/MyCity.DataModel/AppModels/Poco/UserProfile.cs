using System;

namespace MyCity.DataModel.AppModels {
	public class UserProfile {
		public int Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string NationalCode { get; set; }
		public string BirthDate { get; set; }
		public string Address { get; set; }
		public string Email { get; set; }
		public EducationGrade? Grade { get; set; }
		public string Avatar { get; set; }
		public int UserId { get; set; }
		public DateTime CreateDate { get; set; }
		public DateTime? LastModifyDate { get; set; }

		public virtual AppUser User { get; set; }
	}
}
