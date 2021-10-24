using Microsoft.AspNetCore.Identity;
using MyCity.DataModel.AppModels;
using System;
using System.Collections.Generic;

namespace MyCity.DataModel
{
    public class User : IdentityUser<int>
    {
        public AccountType AccountType { get; set; }
		public string ActivationCode { get; set; }
		public bool Activated { get; set; }
		public ExpertStatus ExpertStatus { get; set; }
		public bool IsBlocked { get; set; }
		public DateTime? CreateDate { get; set; }
		public DateTime? LastModify { get; set; }
		public DateTime? LastLogin { get; set; }
	}
}
