using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyCiry.Utilities;
using MyCiry.ViewModel;
using MyCity.DataModel;
using MyCity.DataModel.AppModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyCity.API.Controllers.V1.UserReportApis {
	[Route("/api/v1/[controller]/[action]")]
	public class ReportManager : ControllerBase {
		private readonly IConfiguration _config;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<Role> _roleManager;
		private readonly IMyDataService _iMyDataServ;
		private readonly IWebHostEnvironment _hosting;

		public ReportManager(IConfiguration config,
			SignInManager<User> signInManager,
			RoleManager<Role> roleManager,
			UserManager<User> userManager,
			IMyDataService iMyDataServ,
			IWebHostEnvironment hosting
			) {
			_config = config;
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_iMyDataServ = iMyDataServ;
			_hosting = hosting;
		}

		[HttpPost]
		[Authorize("UserAccess")]
		public async Task<IActionResult> GetTargetAreas([FromBody] TargetAreaSearch request) {

			//var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

			if (request.Part == 0) {
				request.Part = 1;
			}

			if (request.Size == 0 || request.Size > 100) {
				request.Size = 10;
			}

			int take = request.Size;
			int skip = (request.Part - 1) * request.Part;

			var query = _iMyDataServ.iTargetAreaServ.QueryMaker(y => y.Where(x => true));

			if (!string.IsNullOrEmpty(request.Title))
				query = query.Where(x => x.Title.Contains(request.Title));


			var totalCount = query.Count();

			var datas = await query.OrderByDescending(x => x.Id).Skip(skip).Take(take).Select(x => new {
				x.Id,
				x.Title,
				x.Description,
				x.Latlatitude,
				x.Longitude,
				x.CreateBy,
				x.CraeteDate,
				x.ModifyBy,
				x.ModifyDate
			}).ToListAsync();

			var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Mobile";

			return Ok(new {
				message = "لیست مناطق",
				BaseAddress = baseUrl,
				count = totalCount,
				data = datas
			});

		}

		[HttpPost]
		[Authorize("AdminAccess")]
		public async Task<IActionResult> SetTargetArea([FromBody] TargetAreaRequest request) {

			var area = new TargetArea();
			if (request.Id > 0) {
				area = await _iMyDataServ.iTargetAreaServ.FindAsync(x => x.Id == request.Id);
				if (area == null)
					return BadRequest(new {
						Message = "امکان ویرایش این منطقه وجود ندارد"
					});

				area.ModifyBy = request.ModifyBy > 0 ? request.ModifyBy : null;
				area.ModifyDate = DateTime.Now;
			}

			area.Title = request.Title;
			area.Description = request.Description;
			area.Latlatitude = request.Latlatitude;
			area.Longitude = request.Longitude;

			if (request.Id == 0) {
				area.CreateBy = request.CreateBy;
				area.CraeteDate = DateTime.Now;
				_iMyDataServ.iTargetAreaServ.Add(area);
			}

			await _iMyDataServ.iTargetAreaServ.SaveChangesAsync();


			return Ok(new {
				Message = "ثبت موفق گزارش",
				Data = new {
					area.Id,
					area.Title,
					area.Description,
					area.Latlatitude,
					area.Longitude,
					area.CraeteDate,
					area.ModifyDate
				}
			});
		}


		[HttpPost]
		[Authorize("UserAccess")]
		public async Task<IActionResult> GetUserReports([FromBody] UserReportSearch request) {

			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

			if (request.Part == 0) {
				request.Part = 1;
			}

			if (request.Size == 0 || request.Size > 100) {
				request.Size = 10;
			}

			int take = request.Size;
			int skip = (request.Part - 1) * request.Part;

			var query = _iMyDataServ.iReportServ.QueryMaker(y => y.Where(x => x.UserId == user.Id));

			if (request.AreaId > 0)
				query = query.Where(x => x.TargetAreaId == request.AreaId);

			if (request.Status > 0) {
				var status = (ReportStatus) request.Status;
				query = query.Where(x => x.Status == status);
			}


			var totalCount = query.Count();

			var datas = await query.OrderByDescending(x => x.Id).Skip(skip).Take(take).Select(x => new {
				x.Id,
				x.TargetAreaId,
				x.Subject,
				x.UserText,
				x.FileSrc,
				x.FileType,
				x.Status,
				x.UserId,
				x.ExpertId,
				x.InspectorId,
				x.ComplaintCount,
				x.CraeteDate,
				x.ModifyDate,
				ExpertsRecords = x.ReportExperts.Select(y => new {
					y.Id,
					y.ExpertId,
					y.Action,
					y.Description,
					y.CraeteDate
				})
			}).ToListAsync();

			var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Mobile";

			return Ok(new {
				message = "لیست گزارشات کاربر",
				BaseAddress = baseUrl,
				count = totalCount,
				data = datas
			});

		}

		[HttpPost]
		[Authorize("UserAccess")]
		public async Task<IActionResult> SetUserReport([FromBody] UserReportRequest request) {
			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

			var theArea = _iMyDataServ.iTargetAreaServ.GetClosestArea(request.Latlatitude, request.Longitude);

			if (theArea == null) {
				return BadRequest(new {
					Message = "امکان ثبت این گزارش وجود ندارد."
				});
			}

			var report = new Report();
			if (request.Id > 0) {
				report = await _iMyDataServ.iReportServ.FindAsync(x => x.Id == request.Id && x.UserId == user.Id && x.Status == ReportStatus.Registered);
				if (report == null)
					return BadRequest(new {
						Message = "امکان ویرایش این گزارش وجود ندارد"
					});

				report.ModifyDate = DateTime.Now;
			}

			report.TargetAreaId = theArea.Id;
			report.Subject = request.Subject;
			report.UserText = request.UserText;
			report.Latlatitude = request.Latlatitude;
			report.Longitude = request.Longitude;

			if (request.Id == 0) {
				report.Status = ReportStatus.Registered;
				report.UserId = user.Id;
				report.CraeteDate = DateTime.Now;
				_iMyDataServ.iReportServ.Add(report);
			}

			await _iMyDataServ.iReportServ.SaveChangesAsync();

			var reportExperts = _iMyDataServ.iReportExpertServ.GetAll(x => x.ReportId == report.Id);


			return Ok(new {
				Message = "ثبت موفق گزارش",
				Data = new {
					report.Id,
					report.TargetAreaId,
					report.Subject,
					report.UserText,
					report.FileSrc,
					report.FileType,
					report.Status,
					report.UserId,
					report.ExpertId,
					report.InspectorId,
					report.ComplaintCount,
					report.CraeteDate,
					report.ModifyDate,
					ExpertsRecords = reportExperts
				}
			});
		}

		[HttpPost]
		[Authorize("UserAccess")]
		public async Task<IActionResult> ReportFileUpload([FromForm] ReportFileRequest request) {
			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
			var vExs = new string[] { "mp4", "mov", "wmv", "flv", "avi", "webm", "mkv" };
			var iExs = new string[] { "img", "jpeg", "png", "gif" };
			var sExs = new string[] { "mp3", "m4a", "wav", "wma" };

			var fileType = (FileType) request.FileType;

			if (request.File == null) {
				return BadRequest(new { message = "هیچ فایلی انتخاب نشده است." });
			}

			var fexn = Path.GetExtension(request.File.FileName);

			switch (fileType) {
				case FileType.Image:
					if (!iExs.Contains(fexn))
						return BadRequest(new { message = "فرمت فایل انتخابی مجاز نیست." });
					break;
				case FileType.Video:
					if (!vExs.Contains(fexn))
						return BadRequest(new { message = "فرمت فایل انتخابی مجاز نیست." });
					break;
				case FileType.Sound:
					if (!sExs.Contains(fexn))
						return BadRequest(new { message = "فرمت فایل انتخابی مجاز نیست." });
					break;
			}

			string file = string.Empty;
			if (request.File != null && request.File.Length > 0) {
				var saveImage = await Tools.SaveFileAsync(new SaveFileRequest {
					File = request.File,
					RootPath = _hosting.ContentRootPath,
					UnitPath = _config.GetSection("FileRoot:UserReport").Value
				});

				if (!saveImage.Success) {
					return StatusCode(405, new { message = "آپلود تصویر انجام نشد. مجدد تلاش کنید." });
				}

				file = saveImage.FilePath;
			}

			if (request.Id > 0) {
				var report = await _iMyDataServ.iReportServ.FindAsync(x => x.Id == request.Id && x.UserId == user.Id && x.Status == ReportStatus.Registered);
				if (report == null)
					return BadRequest(new {
						Message = "امکان ویرایش این گزارش وجود ندارد"
					});

				if (!string.IsNullOrEmpty(report.FileSrc)) {
					Tools.RemoveFile(new RemoveFileRequest {
						RootPath = _hosting.ContentRootPath + "/" + _config.GetSection("FileRoot:UserReport").Value,
						FilePath = report.FileSrc
					});
				}

				report.ModifyDate = DateTime.Now;
				if (!string.IsNullOrEmpty(file))
					report.FileSrc = file;
				report.FileType = fileType;
				await _iMyDataServ.iReportServ.SaveChangesAsync();
			}

			return Ok(new {
				Message = "بارگزاری فایل انجام شد.",
				Data = new {
					IsDone = true,
					FileUrl = file,
					FileType = request.FileType
				}
			});
		}

		[HttpPost]
		[Authorize("ExpertAccess")]
		public async Task<IActionResult> GetRegisteredReports([FromBody] ReportExpertSearch request) {

			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
			var profile = await _iMyDataServ.iUserProfileServ.FindAsync(x => x.UserId == user.Id);

			if (profile == null || profile.TargetAreaId == null) {
				return BadRequest(new {
					Message = "مراحل ثبت نام شما همکار محترم کامل نشده است."
				});
			}

			if (request.Part == 0) {
				request.Part = 1;
			}

			if (request.Size == 0 || request.Size > 100) {
				request.Size = 10;
			}

			int take = request.Size;
			int skip = (request.Part - 1) * request.Part;

			var query = _iMyDataServ.iReportServ.QueryMaker(y => y.Where(x => x.Status == ReportStatus.Registered && x.TargetAreaId == profile.TargetAreaId));

			if (!string.IsNullOrEmpty(request.Subject))
				query = query.Where(x => x.Subject.Contains(request.Subject));


			var totalCount = query.Count();

			var datas = await query.OrderByDescending(x => x.Id).Skip(skip).Take(take).Select(x => new {
				x.Id,
				x.TargetAreaId,
				x.Subject,
				x.UserText,
				x.FileSrc,
				x.FileType,
				x.Status,
				x.UserId,
				x.ExpertId,
				x.InspectorId,
				x.ComplaintCount,
				x.CraeteDate,
				x.ModifyDate,
				ExpertsRecords = x.ReportExperts.Select(y => new {
					y.Id,
					y.ExpertId,
					y.Action,
					y.Description,
					y.CraeteDate
				})
			}).ToListAsync();

			var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Mobile";

			return Ok(new {
				message = "گزارشات ثبت شده",
				BaseAddress = baseUrl,
				count = totalCount,
				data = datas
			});

		}

		[HttpPost]
		[Authorize("ExpertAccess")]
		public async Task<IActionResult> SetReportToDoByExpert([FromBody] ExpertToDoRequest request) {
			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
			var profile = await _iMyDataServ.iUserProfileServ.FindAsync(x => x.UserId == user.Id);

			if (profile == null || profile.TargetAreaId == null) {
				return BadRequest(new {
					Message = "مراحل ثبت نام شما همکار محترم کامل نشده است."
				});
			}


			var report = await _iMyDataServ.iReportServ.FindAsync(x => x.Id == request.ReportId && x.Status == ReportStatus.Registered && x.TargetAreaId == profile.TargetAreaId);

			if (report == null) {
				return BadRequest(new {
					Message = "امکان انجام عملیات روی این گزارش وجود ندارد."
				});
			}

			report.ExpertId = user.Id;
			report.Status = ReportStatus.ExpertToDo;

			await _iMyDataServ.iReportServ.SaveChangesAsync();


			return Ok(new {
				Message = "گزارش با موقفیت به کارتابل افزوده شد.",
				Data = new {
					report.Id,
					report.TargetAreaId,
					report.Subject,
					report.UserText,
					report.FileSrc,
					report.FileType,
					report.Status,
					report.UserId,
					report.ExpertId,
					report.InspectorId,
					report.ComplaintCount,
					report.CraeteDate,
					report.ModifyDate,
					ExpertsRecords = report.ReportExperts.Select(y => new {
						y.Id,
						y.ExpertId,
						y.Action,
						y.Description,
						y.CraeteDate
					})
				}
			});
		}

		[HttpPost]
		[Authorize("ExpertAccess")]
		public async Task<IActionResult> GetExpertToDoReports([FromBody] ReportExpertSearch request) {

			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

			if (request.Part == 0) {
				request.Part = 1;
			}

			if (request.Size == 0 || request.Size > 100) {
				request.Size = 10;
			}

			int take = request.Size;
			int skip = (request.Part - 1) * request.Part;

			var query = _iMyDataServ.iReportServ.QueryMaker(y => y.Where(x => x.Status == ReportStatus.ExpertToDo && x.ExpertId == user.Id));

			if (!string.IsNullOrEmpty(request.Subject))
				query = query.Where(x => x.Subject.Contains(request.Subject));


			var totalCount = query.Count();

			var datas = await query.OrderByDescending(x => x.Id).Skip(skip).Take(take).Select(x => new {
				x.Id,
				x.TargetAreaId,
				x.Subject,
				x.UserText,
				x.FileSrc,
				x.FileType,
				x.Status,
				x.UserId,
				x.ExpertId,
				x.InspectorId,
				x.ComplaintCount,
				x.CraeteDate,
				x.ModifyDate,
				ExpertsRecords = x.ReportExperts.Select(y => new {
					y.Id,
					y.ExpertId,
					y.Action,
					y.Description,
					y.CraeteDate
				})
			}).ToListAsync();

			var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Mobile";

			return Ok(new {
				message = "گزارشات ثبت شده جهت بررسی",
				BaseAddress = baseUrl,
				count = totalCount,
				data = datas
			});

		}

		[HttpPost]
		[Authorize("ExpertAccess")]
		public async Task<IActionResult> ExpertDoAction([FromBody] ReportExpertRequest request) {
			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);
			var profile = await _iMyDataServ.iUserProfileServ.FindAsync(x => x.UserId == user.Id);

			if (profile == null || profile.TargetAreaId == null) {
				return BadRequest(new {
					Message = "مراحل ثبت نام شما همکار محترم کامل نشده است."
				});
			}


			var report = await _iMyDataServ.iReportServ.FindAsync(x => x.Id == request.ReportId && x.ExpertId == user.Id);

			if (report == null) {
				return BadRequest(new {
					Message = "امکان انجام عملیات روی این گزارش وجود ندارد."
				});
			}

			var action = (ExpertAction) request.Action;

			ReportExpert reportExpert = new ReportExpert();

			if (request.Id > 0) {
				reportExpert = await _iMyDataServ.iReportExpertServ.FindAsync(x => x.Id == request.Id);
				reportExpert.ModifyDate = DateTime.Now;
			}


			reportExpert.Action = action;
			reportExpert.ExpertId = user.Id;
			reportExpert.ReportId = report.Id;
			reportExpert.Description = request.Description;

			if (request.Id == 0) {
				reportExpert.CraeteDate = DateTime.Now;
				_iMyDataServ.iReportExpertServ.Add(reportExpert);
			}

			switch (action) {
				case ExpertAction.Done:
					report.Status = ReportStatus.DoneByExpert;
					break;
				case ExpertAction.Reject:
					report.Status = ReportStatus.RejectByExpert;
					break;
				case ExpertAction.Archive:
					report.Status = ReportStatus.Archive;
					break;
			}

			await _iMyDataServ.iReportServ.SaveChangesAsync();


			return Ok(new {
				Message = "عملیات با موفقیت انجام شد.",
				Data = new {
					reportExpert.Id,
					reportExpert.Description,
					reportExpert.Action,
					reportExpert.ReportId,
					reportExpert.ExpertId,
					reportExpert.CraeteDate,
					reportExpert.ModifyDate
				}
			});
		}

		[HttpPost]
		[Authorize("ExpertAccess")]
		public async Task<IActionResult> GetExpertDones([FromBody] ReportExpertSearch request) {

			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

			if (request.Part == 0) {
				request.Part = 1;
			}

			if (request.Size == 0 || request.Size > 100) {
				request.Size = 10;
			}

			int take = request.Size;
			int skip = (request.Part - 1) * request.Part;

			var query = _iMyDataServ.iReportServ.QueryMaker(y => y.Where(x => x.Status == ReportStatus.DoneByExpert && x.ExpertId == user.Id));

			if (!string.IsNullOrEmpty(request.Subject))
				query = query.Where(x => x.Subject.Contains(request.Subject));


			var totalCount = query.Count();

			var datas = await query.OrderByDescending(x => x.Id).Skip(skip).Take(take).Select(x => new {
				x.Id,
				x.TargetAreaId,
				x.Subject,
				x.UserText,
				x.FileSrc,
				x.FileType,
				x.Status,
				x.UserId,
				x.ExpertId,
				x.InspectorId,
				x.ComplaintCount,
				x.CraeteDate,
				x.ModifyDate,
				ExpertsRecords = x.ReportExperts.Select(y => new {
					y.Id,
					y.ExpertId,
					y.Action,
					y.Description,
					y.CraeteDate
				})
			}).ToListAsync();

			var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Mobile";

			return Ok(new {
				message = "گزارشات بررسی شده کارشناس",
				BaseAddress = baseUrl,
				count = totalCount,
				data = datas
			});

		}

		[HttpPost]
		[Authorize("ExpertAccess")]
		public async Task<IActionResult> GetExpertRejecteds([FromBody] ReportExpertSearch request) {

			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

			if (request.Part == 0) {
				request.Part = 1;
			}

			if (request.Size == 0 || request.Size > 100) {
				request.Size = 10;
			}

			int take = request.Size;
			int skip = (request.Part - 1) * request.Part;

			var query = _iMyDataServ.iReportServ.QueryMaker(y => y.Where(x => x.Status == ReportStatus.RejectByExpert && x.ExpertId == user.Id));

			if (!string.IsNullOrEmpty(request.Subject))
				query = query.Where(x => x.Subject.Contains(request.Subject));


			var totalCount = query.Count();

			var datas = await query.OrderByDescending(x => x.Id).Skip(skip).Take(take).Select(x => new {
				x.Id,
				x.TargetAreaId,
				x.Subject,
				x.UserText,
				x.FileSrc,
				x.FileType,
				x.Status,
				x.UserId,
				x.ExpertId,
				x.InspectorId,
				x.ComplaintCount,
				x.CraeteDate,
				x.ModifyDate,
				ExpertsRecords = x.ReportExperts.Select(y => new {
					y.Id,
					y.ExpertId,
					y.Action,
					y.Description,
					y.CraeteDate
				})
			}).ToListAsync();

			var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Mobile";

			return Ok(new {
				message = "گزارشات رد شده کارشناس",
				BaseAddress = baseUrl,
				count = totalCount,
				data = datas
			});

		}

		[HttpPost]
		[Authorize("ExpertAccess")]
		public async Task<IActionResult> GetExpertArchives([FromBody] ReportExpertSearch request) {

			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

			if (request.Part == 0) {
				request.Part = 1;
			}

			if (request.Size == 0 || request.Size > 100) {
				request.Size = 10;
			}

			int take = request.Size;
			int skip = (request.Part - 1) * request.Part;

			var query = _iMyDataServ.iReportServ.QueryMaker(y => y.Where(x => x.Status == ReportStatus.Archive && x.ExpertId == user.Id));

			if (!string.IsNullOrEmpty(request.Subject))
				query = query.Where(x => x.Subject.Contains(request.Subject));


			var totalCount = query.Count();

			var datas = await query.OrderByDescending(x => x.Id).Skip(skip).Take(take).Select(x => new {
				x.Id,
				x.TargetAreaId,
				x.Subject,
				x.UserText,
				x.FileSrc,
				x.FileType,
				x.Status,
				x.UserId,
				x.ExpertId,
				x.InspectorId,
				x.ComplaintCount,
				x.CraeteDate,
				x.ModifyDate,
				ExpertsRecords = x.ReportExperts.Select(y => new {
					y.Id,
					y.ExpertId,
					y.Action,
					y.Description,
					y.CraeteDate
				})
			}).ToListAsync();

			var baseUrl = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host.Value + "/Mobile";

			return Ok(new {
				message = "گزارشات رد شده کارشناس",
				BaseAddress = baseUrl,
				count = totalCount,
				data = datas
			});

		}

	}
}
