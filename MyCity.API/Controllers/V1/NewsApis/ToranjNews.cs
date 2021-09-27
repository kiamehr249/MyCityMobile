using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MyCiry.ViewModel;
using MyCity.DataModel;
using MyCity.DataModel.ToranjModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyCity.API.Controllers.V1.NewsApis
{
    [Route("/api/v1/[controller]/[action]")]
    [Authorize("UserAccess")]
    public class ToranjNews : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly IToranjServices _iToranjServ;

        public ToranjNews(UserManager<User> userManager, IConfiguration config)
        {
            _userManager = userManager;
            _config = config;
            _iToranjServ = new ToranjServices(_config);
        }

        [HttpPost]
        public IActionResult Agencies([FromBody] Pager request)
        {
            var portalId = Convert.ToInt32(_config.GetSection("ToranjSettings:PortalId").Value);
            var baseAddress = _config.GetSection("ToranjSettings:BaseAddress").Value;

            if (request.Part == 0)
            {
                request.Part = 1;
            }

            if (request.Size == 0 || request.Size > 100)
            {
                request.Size = 10;
            }

            int take = request.Size;
            int skip = (request.Part - 1) * request.Part;

            var data = _iToranjServ.iNewsAgencyServ.QueryMaker(x => x.Where(y => y.Enabled && y.PortalID == portalId)).OrderByDescending(x => x.ID).Skip(skip).Take(take).Select(x => new { 
                x.ID,
                x.Title,
                x.Description,
                x.IconURI,
                x.PictureURI,
                x.PortalID
            }).ToList();
            return Ok(new
            {
                BaseAddress = baseAddress,
                Data = data
            });
        }

        [HttpPost]
        public IActionResult AgencyNews([FromBody] AgencyNewsRequest request)
        {
            var portalId = Convert.ToInt32(_config.GetSection("ToranjSettings:PortalId").Value);
            var baseAddress = _config.GetSection("ToranjSettings:BaseAddress").Value;

            if (request.Part == 0)
            {
                request.Part = 1;
            }

            if (request.Size == 0 || request.Size > 100)
            {
                request.Size = 10;
            }

            int take = request.Size;
            int skip = (request.Part - 1) * request.Part;

            var data = _iToranjServ.iNewsServ.QueryMaker(x => x.Where(y => y.Enabled && y.PortalID == portalId && y.AgencyID == request.AgencyId)).OrderByDescending(x => x.ID).Skip(skip).Take(take).Select(x => new {
                x.ID,
                x.Title,
                x.LeadText,
                x.FullText,
                x.FPicture,
                x.SPicture,
                x.ThumbnailMid,
                x.ThumbnailSmall,
                x.AgencyID,
                x.PortalID
            }).ToList();
            return Ok(new
            {
                BaseAddress = baseAddress,
                Data = data
            });
        }

        [HttpPost]
        public async Task<IActionResult> News([FromBody] NewsReques request)
        {
            var portalId = Convert.ToInt32(_config.GetSection("ToranjSettings:PortalId").Value);
            var baseAddress = _config.GetSection("ToranjSettings:BaseAddress").Value;

            var news = await _iToranjServ.iNewsServ.FindAsync(x => x.ID == request.NewsId);
            return Ok(new
            {
                BaseAddress = baseAddress,
                Data = new
                {
                    news.ID,
                    news.Title,
                    news.LeadText,
                    news.FullText,
                    news.FPicture,
                    news.SPicture,
                    news.ThumbnailMid,
                    news.ThumbnailSmall,
                    news.AgencyID,
                    news.PortalID
                }
            });
        }

    }
}
