using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyCiry.Utilities;
using MyCiry.ViewModel.Users;
using MyCity.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCity.API.Controllers.V1.UserApis
{
    [Route("/api/v1/[controller]/[action]")]
    //[Authorize(Roles = "Admin")]
    public class UserManager : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;

        public UserManager(IConfiguration config,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager, 
            UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }
        
        [Authorize("AdminAccess")]
        [HttpPost]
        public async Task<IActionResult> SetUserRole([FromBody] UserRoleRequest request)
        {
            var messages = new List<string>();

            if (request.UserId == 0)
            {
                messages.Add("شناسه کاربر نمی تواند خالی باشد");
            }

            if (string.IsNullOrEmpty(request.RoleName))
            {
                messages.Add("عنوان نقش نمی تواند خالی باشد");
            }

            if (messages.Count() > 0)
            {
                return BadRequest(new { Messages = messages });
            }

            var user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
            {
                messages.Add("کاربر یافت نشد");
                return BadRequest(new { Messages = messages });
            }

            var res = await _userManager.AddToRoleAsync(user, request.RoleName);
            if (!res.Succeeded)
            {
                return Ok(new { Messages = res.Errors.Select(x => x.Description).ToList() });
            }

            return Ok(new { IsDone = true });

        }

        [Authorize("AdminAccess")]
        [HttpPost]
        public async Task<IActionResult> SetRole([FromBody] RoleRequest request)
        {
            var messages = new List<string>();

            if (string.IsNullOrEmpty(request.Name))
            {
                messages.Add("عنوان نقش نمی تواند خالی باشد");
            }

            if (messages.Count() > 0)
            {
                return BadRequest(new { Messages = messages });
            }

            var user = await _roleManager.FindByNameAsync(request.Name);

            if (user != null)
            {
                messages.Add("این نقش موجود می باشد");
                return BadRequest(new { Messages = messages });
            }

            var res = await _roleManager.CreateAsync(new Role { 
                Name = request.Name
            });
            if (!res.Succeeded)
            {
                return Ok(new { Messages = res.Errors.Select(x => x.Description).ToList() });
            }

            return Ok(new { IsDone = true });

        }

    }
}
