using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MyCiry.Utilities;
using MyCiry.ViewModel;
using MyCiry.ViewModel.SMS;
using MyCiry.ViewModel.Users;
using MyCity.API.Services.SMS;
using MyCity.DataModel;
using MyCity.DataModel.AppModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyCity.API.Controllers.V1.UserApis {
	[Route("/api/v1/[controller]/[action]")]
	public class Account : ControllerBase {
		private readonly IConfiguration _config;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<Role> _roleManager;
		private readonly ISmsService _iSmsService;
		private readonly IMyDataService _iMyDataServ;
		private readonly IWebHostEnvironment _hosting;
		//private readonly IHttpContextAccessor _httpContextAccessor;

		public Account(
			UserManager<User> userManager, IConfiguration config,
			IWebHostEnvironment hostingEnvironment,
			SignInManager<User> signInManager, RoleManager<Role> roleManager,
			ISmsService iSmsService, IMyDataService iMyDataServ
			) {
			_userManager = userManager;
			_hosting = hostingEnvironment;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_config = config;
			_iSmsService = iSmsService;
			_iMyDataServ = iMyDataServ;
		}

		[HttpPost]
		public async Task<IActionResult> GetToken([FromBody] TokenRequest request) {
			var user = await _userManager.FindByNameAsync(request.Username);

			if (user == null) {
				return StatusCode(400, new { message = "این کاربری یافت نشد", data = new { } });
			}

			if (!user.Activated) {
				return StatusCode(403, new { message = "این کاربری فعال نشده است", data = new { } });
			}

			var isTrust = await _userManager.CheckPasswordAsync(user, request.Password);

			if (!isTrust) {
				return StatusCode(401, new { message = "رمز عبور یا نام کاربری اشتباه است", data = new { } });
			}

			var userClaims = new List<Claim>
				{
					new Claim("Id", user.Id.ToString()),
					new Claim(ClaimTypes.Name, user.UserName)
				};

			var userRoles = await _userManager.GetRolesAsync(user);
			foreach (var role in userRoles) {
				userClaims.Add(new Claim(ClaimTypes.Role, role));
			}

			int lifeDaies = Convert.ToInt32(_config["TokenOptions:LifeDaies"]);
			DateTime nowTime = DateTime.Now;
			DateTime nowUtcTime = DateTime.UtcNow;
			// Creates the signed JWT
			var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenOptions:Key"]));
			var tokenHandler = new JwtSecurityTokenHandler();
			var tokenDescriptor = new SecurityTokenDescriptor {
				Subject = new ClaimsIdentity(userClaims),
				Expires = nowUtcTime.AddDays(lifeDaies),
				Issuer = "ysp24.ir",
				Audience = "ysp24.ir",
				SigningCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var accessToken = tokenHandler.WriteToken(token);

			// Returns the 'access_token' and the type in lower case
			return Ok(new {
				message = "دریافت موفق",
				data = new {
					create = nowTime.ToString(),
					expire = nowTime.AddDays(lifeDaies).ToString(),
					token = accessToken,
					type = "bearer"
				}

			});
		}

		[HttpPost]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request) {
			if (string.IsNullOrEmpty(request.PhoneNumber)) {
				return BadRequest(new { message = "شماره تلفن نمی تواند خالی باشد", data = new { } });
			}

			//if (string.IsNullOrEmpty(request.Password)) {
			//	return BadRequest(new { message = "رمز عبور نمی تواند خالی باشد", data = new { } });
			//}

			//if (string.IsNullOrEmpty(request.ConfirmPassword)) {
			//	return BadRequest(new { message = "تکرار رمز عبور نباید خالی باشد", data = new { } });
			//}

			//if (request.Password != request.ConfirmPassword) {
			//	return BadRequest(new { message = "رمز عبور و تکرار یکسان نمی باشد", data = new { } });
			//}

			request.PhoneNumber = request.PhoneNumber.PersianToEnglish();

			var user = await _userManager.FindByNameAsync(request.PhoneNumber);

			var code = Tools.RandomNumber(5);

			if (user == null) {
				user = new User {
					UserName = request.PhoneNumber,
					Email = "fake." + request.PhoneNumber + "@fmail.com",
					PhoneNumber = request.PhoneNumber,
					EmailConfirmed = true,
					PhoneNumberConfirmed = true,
					AccountType = AccountType.AppUser,
					ActivationCode = code,
					Activated = false,
					CreateDate = DateTime.Now,
					ExpertStatus = ExpertStatus.NotExpert
				};
				var result = await _userManager.CreateAsync(user, "c@" + request.PhoneNumber);
				if (result.Succeeded) {
					await _userManager.AddToRoleAsync(user, "User");
					await _iSmsService.SendSms(new MyCiry.ViewModel.SMS.SendSmsRequest {
						Text = string.Format("کد فعال سازی شما در سامانه خدمات شهری خوی: {0}", code),
						Mobile = user.PhoneNumber
					});
					return Ok(new {
						message = "ثبت نام موفق",
						data = new {
							isDone = true,
							Code = code
						}
					});
				} else {
					string messages = "";
					if (result.Errors.Count() > 0) {
						foreach (var error in result.Errors) {
							messages += " - " + error.Description;
						}
					}
					return BadRequest(new { message = messages, data = "" });
				}
			} else {
				user.ActivationCode = code;
				user.Activated = false;
				user.LastModify = DateTime.Now;
				await _userManager.UpdateAsync(user);

				await _iSmsService.SendSms(new MyCiry.ViewModel.SMS.SendSmsRequest {
					Text = string.Format("کد فعال سازی شما در سامانه خدمات شهری خوی: {0}", code),
					Mobile = user.PhoneNumber
				});
				return Ok(new {
					message = "این کاربری قبلا ثبت نام است",
					data = new {
						isDone = false,
						Code = code
					}
				});
			}



		}

		[HttpPost]
		public async Task<IActionResult> ExpertRegister([FromBody] RegisterRequest request) {
			if (string.IsNullOrEmpty(request.PhoneNumber)) {
				return BadRequest(new { message = "شماره تلفن نمی تواند خالی باشد", data = new { } });
			}

			//if (string.IsNullOrEmpty(request.Password)) {
			//	return BadRequest(new { message = "رمز عبور نمی تواند خالی باشد", data = new { } });
			//}

			//if (string.IsNullOrEmpty(request.ConfirmPassword)) {
			//	return BadRequest(new { message = "تکرار رمز عبور نباید خالی باشد", data = new { } });
			//}

			//if (request.Password != request.ConfirmPassword) {
			//	return BadRequest(new { message = "رمز عبور و تکرار یکسان نمی باشد", data = new { } });
			//}

			request.PhoneNumber = request.PhoneNumber.PersianToEnglish();

			var user = await _userManager.FindByNameAsync(request.PhoneNumber);
			var code = Tools.RandomNumber(5);

			if (user == null) {
				user = new User {
					UserName = request.PhoneNumber,
					Email = "fake." + request.PhoneNumber + "@fmail.com",
					PhoneNumber = request.PhoneNumber,
					EmailConfirmed = true,
					PhoneNumberConfirmed = true,
					AccountType = AccountType.Expert,
					ActivationCode = code,
					Activated = false,
					CreateDate = DateTime.Now,
					ExpertStatus = ExpertStatus.Requested
				};
				var result = await _userManager.CreateAsync(user, "c@" + request.PhoneNumber);
				if (result.Succeeded) {
					await _userManager.AddToRoleAsync(user, "Expert");
					await _iSmsService.SendSms(new MyCiry.ViewModel.SMS.SendSmsRequest {
						Text = string.Format("کد فعال سازی شما در سامانه خدمات شهری خوی: {0}", code),
						Mobile = user.PhoneNumber
					});
					return Ok(new { IsDone = true });
				} else {
					string messages = "";
					if (result.Errors.Count() > 0) {
						foreach (var error in result.Errors) {
							messages += " - " + error.Description;
						}
					}
					return BadRequest(new { message = messages, data = "" });
				}
			} else {
				user.ActivationCode = code;
				user.Activated = false;
				user.LastModify = DateTime.Now;
				await _userManager.UpdateAsync(user);
				await _iSmsService.SendSms(new MyCiry.ViewModel.SMS.SendSmsRequest {
					Text = string.Format("کد فعال سازی شما در سامانه خدمات شهری خوی: {0}", code),
					Mobile = user.PhoneNumber
				});
				return Ok(new {
					message = "این کاربری قبلا ثبت نام است",
					data = new {
						isDone = false,
						Code = code
					}
				});
			}
		}

		[HttpPost]
		public async Task<IActionResult> ConfirmCode([FromBody] ConfirmCodeRequest request) {
			if (string.IsNullOrEmpty(request.Code)) {
				return BadRequest(new { message = "شماره تلفن نمی تواند خالی باشد", data = new { } });
			}

			var user = await _userManager.FindByNameAsync(request.Mobile);

			if (user == null) {
				return BadRequest(new { message = "این کاربری یافت نشد", data = new { } });
			}

			if (user.ActivationCode != request.Code) {
				return BadRequest(new { message = "کد را صحیح وارد کنید", data = new { } });
			}

			user.Activated = true;
			user.LastLogin = DateTime.Now;
			await _userManager.UpdateAsync(user);

			int lifeDaies = Convert.ToInt32(_config["TokenOptions:LifeDaies"]);
			var key = _config["TokenOptions:Key"];
			var roles = await _userManager.GetRolesAsync(user);
			var token = Tools.GenerateToken(user, roles, lifeDaies, key);

			return Ok(new {
				message = "فعال سازی موفق",
				data = new {
					isDone = true,
					token = new {
						create = DateTime.Now.ToString(),
						expire = DateTime.Now.AddDays(lifeDaies).ToString(),
						token = token,
						type = "bearer"
					}
				}
			});
		}


		[HttpPost]
		[Authorize("UserAccess")]
		public async Task<IActionResult> UserProfile([FromBody] UserProfileRequest request) {

			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
			var theProfile = await _iMyDataServ.iUserProfileServ.FindAsync(x => x.UserId == user.Id);

			if (theProfile == null) {
				theProfile = new UserProfile();
			}

			theProfile.FirstName = request.FirstName;
			theProfile.LastName = request.LastName;
			theProfile.Email = request.Email;
			theProfile.NationalCode = request.NationalCode;
			theProfile.BirthDate = request.BirthDate;
			theProfile.Address = request.Address;
			theProfile.Grade = (EducationGrade) request.Grade;

			if (theProfile.Id == 0) {
				theProfile.UserId = user.Id;
				theProfile.CreateDate = DateTime.Now;
				_iMyDataServ.iUserProfileServ.Add(theProfile);
			} else {
				theProfile.LastModifyDate = DateTime.Now;
			}

			await _iMyDataServ.iUserProfileServ.SaveChangesAsync();

			return Ok(new { 
				Message = "ثبت اطلاعات کاربری",
				Data = new {
					UserId = user.Id,
					user.PhoneNumber,
					theProfile.Id,
					theProfile.FirstName,
					theProfile.LastName,
					theProfile.NationalCode,
					theProfile.Email,
					theProfile.Address,
					theProfile.BirthDate,
					theProfile.CreateDate,
					Grade = theProfile.Grade != null ? (int) theProfile.Grade : 0,
					Avatar = string.IsNullOrEmpty(theProfile.Avatar) ? null : HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Mobile/" + theProfile.Avatar,
					theProfile.LastModifyDate,
					IsDone = true
				}
			});

		}

		[HttpGet]
		[Authorize("UserAccess")]
		public async Task<IActionResult> UserProfile() {

			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
			var theProfile = await _iMyDataServ.iUserProfileServ.FindAsync(x => x.UserId == user.Id);

			if (theProfile == null) {
				return Ok(new { 
					Message = "اطلاعات پروفایل کاربری",
					Data = new { }
				});
			}

			return Ok(new {
				Message = "اطلاعات پروفایل کاربری",
				Data = new {
					UserId = user.Id,
					user.PhoneNumber,
					theProfile.Id,
					theProfile.FirstName,
					theProfile.LastName,
					theProfile.NationalCode,
					theProfile.Email,
					theProfile.Address,
					theProfile.BirthDate,
					theProfile.CreateDate,
					Grade = theProfile.Grade != null ? (int) theProfile.Grade : 0,
					Avatar = string.IsNullOrEmpty(theProfile.Avatar) ? null : HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Mobile/" + theProfile.Avatar,
					theProfile.LastModifyDate
				}
			});

		}

		[HttpPost]
		[Authorize("UserAccess")]
		public async Task<IActionResult> SetUserAvatat([FromForm]UserAvatatRequest request) {

			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
			var theProfile = await _iMyDataServ.iUserProfileServ.FindAsync(x => x.UserId == user.Id);

			if (request.Avatar == null) {
				return BadRequest(new { message = "هیچ فایلی انتخاب نشده است." });
			}

			if (theProfile != null && !string.IsNullOrEmpty(theProfile.Avatar)) {
				Tools.RemoveFile(new RemoveFileRequest { 
					RootPath = _hosting.ContentRootPath + "/" + _config.GetSection("FileRoot:UserAvatar").Value,
					FilePath = theProfile.Avatar
				});
			}

			string image = string.Empty;
			if (request.Avatar != null && request.Avatar.Length > 0) {
				var saveImage = await Tools.SaveFileAsync(new SaveFileRequest {
					File = request.Avatar,
					RootPath = _hosting.ContentRootPath,
					UnitPath = _config.GetSection("FileRoot:UserAvatar").Value
				});

				if (!saveImage.Success) {
					return StatusCode(405, new { message = "آپلود تصویر انجام نشد. مجدد تلاش کنید." });
				}

				image = saveImage.FilePath;
			}

			if(theProfile == null) {
				theProfile = new UserProfile();
				theProfile.UserId = user.Id;
				theProfile.CreateDate = DateTime.Now;
			} else {
				theProfile.LastModifyDate = DateTime.Now;
			}

			theProfile.Avatar = image;

			if (theProfile.Id == 0) {
				_iMyDataServ.iUserProfileServ.Add(theProfile);
			}

			await _iMyDataServ.iUserProfileServ.SaveChangesAsync();

			return Ok(new {
				Message = "تصویر چهره کاربر",
				Data = new {
					UserId = user.Id,
					ProfileId = theProfile.Id,
					Avatar = string.IsNullOrEmpty(image) ? null : HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Mobile/" + theProfile.Avatar
				}
			});

		}

		[HttpPost]
		public async Task<IActionResult> SendSms([FromBody] SendSmsRequest request) {
			var result = await _iSmsService.SendSms(request);
			return Ok(result);
		}

		[HttpPost]
		public async Task<IActionResult> GetLines() {
			var result = await _iSmsService.GetSmsLines();
			return Ok(result);
		}

	}
}
