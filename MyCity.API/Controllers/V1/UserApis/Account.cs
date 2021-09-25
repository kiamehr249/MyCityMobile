using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyCiry.Utilities;
using MyCiry.ViewModel.Users;
using MyCity.DataModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyCity.API.Controllers.V1.UserApis
{
    [Route("/api/v1/[controller]/[action]")]
    public class Account : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;

        public Account(UserManager<User> userManager, IConfiguration config, 
            SignInManager<User> signInManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> GetToken([FromBody] TokenRequest request)
        {
            DataModel.User user = await _userManager.FindByNameAsync(request.Username);

            if (user == null)
            {
                return Ok(new { status = 403, message = "access denaid" });
            }

            var isTrust = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isTrust)
            {
                return Ok(new { status = 403, message = "access denaid" });
            }

            int lifeDaies = Convert.ToInt32(_config["TokenOptions:LifeDaies"]);
            DateTime nowTime = DateTime.Now;
            DateTime nowUtcTime = DateTime.UtcNow;
            // Creates the signed JWT
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenOptions:Key"]));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("id", user.Id.ToString())
                }),
                Expires = nowUtcTime.AddDays(lifeDaies),
                Issuer = "niksoftgroup.ir",
                Audience = "niksoftgroup.ir",
                SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);

            // Returns the 'access_token' and the type in lower case
            return Ok(new
            {
                create = nowTime.ToString(),
                expair = nowTime.AddDays(lifeDaies).ToString(),
                token = accessToken,
                type = "bearer"
            });
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var errorMessages = new List<string>();

            if (string.IsNullOrEmpty(request.PhoneNumber))
            {
                errorMessages.Add("شماره تلفن نمی تواند خالی باشد");
            }

            if (string.IsNullOrEmpty(request.Password))
            {
                errorMessages.Add("رمز عبور نمی تواند خالی باشد");
            }

            if (string.IsNullOrEmpty(request.ConfirmPassword))
            {
                errorMessages.Add("تکرار رمز عبور نباید خالی باشد");
            }

            if (request.Password != request.ConfirmPassword)
            {
                errorMessages.Add("رمز عبور و تکرار یکسان نمی باشد");
            }

            if (errorMessages.Count() > 0)
            {
                return BadRequest(new { Messages = errorMessages });
            }

            request.PhoneNumber = request.PhoneNumber.PersianToEnglish();

            var userCheck = await _userManager.FindByNameAsync(request.PhoneNumber);
            if (userCheck == null)
            {
                var user = new User
                {
                    UserName = request.PhoneNumber,
                    Email = "fake." + request.PhoneNumber + "@fmail.com",
                    PhoneNumber = request.PhoneNumber,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    return Ok(new { IsDone = true });
                }
                else
                {
                    if (result.Errors.Count() > 0)
                    {
                        foreach (var error in result.Errors)
                        {
                            errorMessages.Add(error.Description);
                        }
                    }
                    return StatusCode(500, new { Messages = errorMessages });
                }
            }
            else
            {
                errorMessages.Add("این کاربری قبلا ثبت نام است");
                return StatusCode(500, new { Messages = errorMessages });
            }
        }


    }
}
