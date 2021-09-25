using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyCity.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCity.API.Controllers.V1.UserApis
{
    [Route("/api/v1/[controller]/[action]")]
    public class UserManager : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserManager(IConfiguration config,
            SignInManager<User> signInManager, 
            UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult GetData()
        {
            return Ok(new { test = true });
        }
    }
}
