using Microsoft.AspNetCore.Identity;
using MyCity.DataModel.AppModels;
using System.Collections.Generic;

namespace MyCity.DataModel
{
    public class User : IdentityUser<int>
    {
        public AccountType AccountType { get; set; }
		public string ActivationCode { get; set; }
		public bool Activated { get; set; }

		public virtual ICollection<UserProfile> UserProfiles { get; set; }
	}
}
