using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCity.DataModel.AppModels {
	public interface IMyDataService {
		IUserProfileService iUserProfileServ { get; set; }
	}

	public class MyDataService : IMyDataService {
		IConfiguration config;
		public IUserProfileService iUserProfileServ { get; set; }

		public MyDataService(IConfiguration config) {
			IMyUnitOfWork uow = new MyDbContext(config.GetConnectionString("SystemBase"));
			iUserProfileServ = new UserProfileService(uow);
		}



	}
}
