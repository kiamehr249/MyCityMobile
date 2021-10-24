using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyCiry.Utilities;
using MyCiry.ViewModel;
using MyCiry.ViewModel.Users;
using MyCity.DataModel;
using MyCity.DataModel.AppModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCity.API.Controllers.V1.UserApis
{
    [Route("/api/v1/[controller]/[action]")]
    //[Authorize("AdminAccess")]
	public class UserManager : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
		private readonly IMyDataService _iMyDataServ;

		public UserManager(IConfiguration config,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager, 
            UserManager<User> userManager,
			IMyDataService iMyDataServ
			)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
			_iMyDataServ = iMyDataServ;
		}
        
        [HttpPost]
        public async Task<IActionResult> SetUserRole([FromBody] UserRoleRequest request)
        {
            if (request.UserId == 0)
            {
				return BadRequest(new {
					message = "شناسه کاربر نمی تواند خالی باشد",
					data = false
				});
			}

            if (string.IsNullOrEmpty(request.RoleName))
            {
				return BadRequest(new {
					message = "عنوان نقش نمی تواند خالی باشد",
					data = false
				});
			}

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
            {
				return BadRequest(new {
					message = "کاربر یافت نشد",
					data = false
				});
			}

            var res = await _userManager.AddToRoleAsync(user, request.RoleName);
            if (!res.Succeeded)
            {
				var messages = "";
				foreach (var error in res.Errors) {
					messages += ", " + error.Description;
				}
				return Ok(new {
					message = messages,
					data = false
				});
			}

			return Ok(new {
				message = "نقش با موفقیت به کاربر افزوده شد",
				data = true
			});

		}

        [HttpPost]
        public async Task<IActionResult> SetRole([FromBody] RoleRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
				return BadRequest(new { 
					message = "عنوان نقش نمی تواند خالی باشد", 
					data = false 
				});
			}

            var user = await _roleManager.FindByNameAsync(request.Name);

            if (user != null)
            {
				return BadRequest(new {
					message = "این نقش موجود می باشد",
					data = false
				});
			}

            var res = await _roleManager.CreateAsync(new Role { 
                Name = request.Name
            });

            if (!res.Succeeded)
            {
				var messages = "";
				foreach (var error in res.Errors) {
					messages += ", " + error.Description;
				}
                return Ok(new { 
					message = messages, 
					data = false 
				});
            }

            return Ok(new { 
				message = "نقش با موفقیت افزوده شد", 
				data = true 
			});

        }

		[HttpPost]
		public async Task<IActionResult> GetUsers([FromBody] GetUserRequest request) {

			if (request.Part == 0) {
				request.Part = 1;
			}

			if (request.Size == 0 || request.Size > 100) {
				request.Size = 10;
			}

			int take = request.Size;
			int skip = (request.Part - 1) * request.Part;

			var query = _iMyDataServ.iAppUserServ.QueryMaker(y => y.Where(x => true));

			if (request.AccountType != null) {
				query = query.Where(x => x.AccountType == (AccountType) request.AccountType);
			}

			if (request.Activated != null) {
				query = query.Where(x => x.Activated == request.Activated);
			}

			if (request.ExpertStatus != null) {
				query = query.Where(x => x.ExpertStatus == (ExpertStatus) request.ExpertStatus);
			}

			if (!string.IsNullOrEmpty(request.UserName)) {
				query = query.Where(x => x.UserName.Contains(request.UserName));
			}

			var totalCount = await query.CountAsync();

			var users = await query.Include(x => x.UserProfiles).Skip(skip).Take(take).Select(x => new { 
				x.Id,
				x.UserName,
				x.PhoneNumber,
				x.AccountType,
				x.Activated,
				x.ActivationCode,
				x.ExpertStatus,
				x.IsBlocked,
				x.CreateDate,
				x.LastModify,
				x.LastLogin,
				Profile = x.UserProfiles.Select(y => new { 
					y.Id,
					y.FirstName,
					y.LastName,
					y.Avatar,
					y.BirthDate,
					y.Address,
					y.Email,
					y.NationalCode,
					y.CreateDate,
					y.LastModifyDate
				})
			}).ToListAsync();

			return Ok(new { 
				message = "لیست کاربران",
				count = totalCount,
				data = users 
			});

		}

		[HttpPost]
		public async Task<IActionResult> GetUserRoles([FromBody] GetUserRolesRequest request) {

			if (request.Part == 0) {
				request.Part = 1;
			}

			if (request.Size == 0 || request.Size > 100) {
				request.Size = 10;
			}

			int take = request.Size;
			int skip = (request.Part - 1) * request.Part;

			var user = await _userManager.FindByIdAsync(request.UserId.ToString());
			var roles = await _userManager.GetRolesAsync(user);

			return Ok(new {
				message = "لیست کاربران",
				data = roles
			});

		}


		[HttpPost]
		public async Task<IActionResult> RemoveUserRole([FromBody] UserRoleRequest request) {

			if (string.IsNullOrEmpty(request.RoleName) || request.UserId == 0) {
				return BadRequest(new {
					message = "مقادیر ورودی را چک کنید",
					data = false
				});
			}


			var user = await _userManager.FindByIdAsync(request.UserId.ToString());
			if (user == null) {
				return BadRequest(new {
					message = "چنین کاربری وجود ندارد",
					data = false
				});
			}

			await _userManager.RemoveFromRoleAsync(user, request.RoleName);

			return Ok(new {
				message = "حذف نقش با موفقیت انجام شد",
				data = true
			});

		}


		[HttpPost]
		public async Task<IActionResult> ConfirmExpert([FromBody] ConfirmExpertRequest request) {
			var messages = new List<string>();

			if (request.UserId == 0) {
				return BadRequest(new { 
					message = "شناسه کاربری نمی تواند خالی باشد", 
					data = "" 
				});
			}

			var user = await _userManager.FindByIdAsync(request.UserId.ToString());

			if (user == null) {
				return BadRequest(new {
					message = "این کاربری موجود نمی باشد",
					data = ""
				});
			}

			user.ExpertStatus = (ExpertStatus) request.ExpertStatus;
			await _userManager.UpdateAsync(user);

			return Ok(new { 
				message = "ثبت موفق تغییرات",
				data = request.ExpertStatus
			});

		}


		[HttpPost]
		public async Task<IActionResult> BlockUser([FromBody] BlockUserRequest request) {
			if (request.UserId == 0) {
				return BadRequest(new {
					message = "شناسه کاربر نمی تواند خالی باشد",
					data = ""
				});
			}

			var user = await _userManager.FindByIdAsync(request.UserId.ToString());

			if (user == null) {
				return BadRequest(new {
					message = "کاربر یافت نشد",
					data = ""
				});
			}

			user.IsBlocked = request.Blocked;
			await _userManager.UpdateAsync(user);

			return Ok(new {
				message = "عملیات انجام شد",
				data = request.Blocked
			});

		}


	}
}
