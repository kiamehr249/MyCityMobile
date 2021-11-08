using System;
using System.Collections.Generic;

namespace MyCity.DataModel.AppModels {
	public class AppUser {
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Email { get; set; }
		public bool EmailConfirmed { get; set; }
		public string PhoneNumber { get; set; }
		public string PasswordHash { get; set; }
		public bool PhoneNumberConfirmed { get; set; }
		public AccountType AccountType { get; set; }
		public string ActivationCode { get; set; }
		public bool Activated { get; set; }
		public ExpertStatus ExpertStatus { get; set; }
		public bool IsBlocked { get; set; }
		public DateTime? CreateDate { get; set; }
		public DateTime? LastModify { get; set; }
		public DateTime? LastLogin { get; set; }

		public virtual ICollection<UserProfile> UserProfiles { get; set; }
		public virtual ICollection<UserRate> UserRates { get; set; }
	}
}
