using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyCiry.ViewModel;
using MyCity.DataModel;
using MyCity.DataModel.ToranjModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCity.API.Controllers.V1.PollApis {
	[Route("/api/v1/[controller]/[action]")]
	[Authorize("UserAccess")]
	public class ToranjPoll : ControllerBase {

		private readonly IConfiguration _config;
		private readonly UserManager<User> _userManager;
		private readonly IToranjServices _iToranjServ;

		public ToranjPoll(UserManager<User> userManager, IConfiguration config, IToranjServices iToranjServ) {
			_userManager = userManager;
			_config = config;
			_iToranjServ = iToranjServ;
		}

		[HttpPost]
		public IActionResult Categories([FromBody] Pager request) {
			var portalId = Convert.ToInt32(_config.GetSection("ToranjSettings:PortalId").Value);
			var baseAddress = _config.GetSection("ToranjSettings:BaseAddress").Value;

			if (request.Part == 0) {
				request.Part = 1;
			}

			if (request.Size == 0 || request.Size > 100) {
				request.Size = 10;
			}

			int take = request.Size;
			int skip = (request.Part - 1) * request.Part;

			var data = _iToranjServ.iPollCategoryServ.QueryMaker(x => x.Where(y => y.Enabled && y.PortalID == portalId)).OrderByDescending(x => x.ID).Skip(skip).Take(take).Select(x => new {
				x.ID,
				x.Title,
				x.ImgUrl,
				x.PortalID
			}).ToList();
			return Ok(new {
				Message = "دسته های نظرسنجی",
				BaseAddress = baseAddress,
				Data = data
			});
		}

		[HttpPost]
		public IActionResult CategoryPolls([FromBody] CategoryPollRequest request) {
			var portalId = Convert.ToInt32(_config.GetSection("ToranjSettings:PortalId").Value);
			var baseAddress = _config.GetSection("ToranjSettings:BaseAddress").Value;

			if (request.Part == 0) {
				request.Part = 1;
			}

			if (request.Size == 0 || request.Size > 100) {
				request.Size = 10;
			}

			int take = request.Size;
			int skip = (request.Part - 1) * request.Part;

			var query = _iToranjServ.iPollServ.QueryMaker(x => x.Where(y => y.Enabled && y.PortalID == portalId));

			if (request.CategoryId > 0) {
				query = query.Where(x => x.CategoryID == request.CategoryId);
			}

			var data = query.OrderByDescending(x => x.ID).Skip(skip).Take(take).Select(x => new {
				x.ID,
				x.Title,
				x.PortalID,
				PollQuestions = x.PollQuestions.Select(x => new {
					x.ID,
					x.Title,
					x.SelectedCount
				})
			}).ToList();
			return Ok(new {
				Message = "نظرسنجی ها",
				BaseAddress = baseAddress,
				Data = data
			});
		}


		[HttpPost]
		public async Task<IActionResult> SetAnsware([FromBody] PollAnswerRequest request) {
			var portalId = Convert.ToInt32(_config.GetSection("ToranjSettings:PortalId").Value);
			var baseAddress = _config.GetSection("ToranjSettings:BaseAddress").Value;
			var user = await _userManager.FindByIdAsync(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "Id").Value);

			if (request.PollId == 0) {
				return BadRequest(new { message = "شناسه نظرسنجی خالیست" });
			}

			if (request.QuestionId == 0) {
				return BadRequest(new { message = "هیچ گزینه ای انتخاب نشده است" });
			}

			var strUserId = user.Id.ToString();
			var oldAnswer = await _iToranjServ.iPollQuestionAnswerServ.CountAsync(x => x.UserAnswer == strUserId);
			if (oldAnswer > 0) {
				return StatusCode(403, new { message = "شما قبلا در این نظرسنجی شرکت کرده اید." });
			}

			var theQuestion = await _iToranjServ.iPollQuestionServ.FindAsync(x => x.ID == request.QuestionId);
			theQuestion.SelectedCount = theQuestion.SelectedCount + 1;

			_iToranjServ.iPollQuestionAnswerServ.Add(new PollQuestionAnswer {
				PollID = request.PollId,
				QuestionID = request.QuestionId,
				UserAnswer = user.Id.ToString(),
				UserIP = HttpContext.Connection.RemoteIpAddress.ToString()
			});

			await _iToranjServ.iPollQuestionAnswerServ.SaveChangesAsync();

			return Ok(new {
				Message = "پاسخ با موفقیت ثبت شد",
				BaseAddress = baseAddress,
				Data = true
			});
		}


	}
}
