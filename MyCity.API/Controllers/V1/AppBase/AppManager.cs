using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyCiry.ViewModel.Base;
using MyCity.API.Services.SMS;
using MyCity.DataModel;
using MyCity.DataModel.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCity.API.Controllers.V1.AppBase {
	[Route("/api/v1/[controller]/[action]")]
	public class AppManager : ControllerBase {
		private readonly IConfiguration _config;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<Role> _roleManager;
		private readonly ISmsService _iSmsService;
		private readonly IMyDataService _iMyDataServ;

		public AppManager(
			UserManager<User> userManager, IConfiguration config,
			SignInManager<User> signInManager, RoleManager<Role> roleManager,
			ISmsService iSmsService, IMyDataService iMyDataServ
			) {
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_config = config;
			_iSmsService = iSmsService;
			_iMyDataServ = iMyDataServ;
		}

		[HttpPost]
		[Authorize("AdminAccess")]
		public async Task<IActionResult> SetVersion([FromBody] AppReleaseRequest request) {
			//var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
			var item = new AppRelease();
			if (request.Id > 0) {
				item = await _iMyDataServ.iAppReleaseServ.FindAsync(x => x.Id == request.Id);
			}

			item.VersionName = request.VersionName;
			item.VersionNo = request.VersionNo;
			item.Url = request.Url;

			if (request.Id == 0) {
				item.CreateDate = DateTime.Now;
				_iMyDataServ.iAppReleaseServ.Add(item);
			}

			await _iMyDataServ.iAppReleaseServ.SaveChangesAsync();

			return Ok(new {
				message = "ثبت موفق",
				data = item

			});
		}

		[HttpGet]
		[Authorize("AdminAccess")]
		public async Task<IActionResult> GetVersion() {
			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

			var data = await _iMyDataServ.iAppReleaseServ.QueryMaker(y => y.Where(x => true)).OrderByDescending(x => x.Id).Select(x => new { 
				x.Id,
				x.VersionName,
				x.VersionNo,
				x.Url,
				x.CreateDate
			}).ToListAsync();

			return Ok(new {
				message = "دریافت موفق",
				data = data

			});
		}


	}
}
