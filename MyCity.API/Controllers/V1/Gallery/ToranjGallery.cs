using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MyCiry.ViewModel;
using MyCity.DataModel;
using MyCity.DataModel.ToranjModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyCity.API.Controllers.V1.Gallery
{
    [Route("/api/v1/[controller]/[action]")]
    [Authorize("UserAccess")]
    public class ToranjGallery : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly IToranjServices _iToranjServ;

        public ToranjGallery(UserManager<User> userManager, IConfiguration config, IToranjServices iToranjServ)
        {
            _userManager = userManager;
            _config = config;
            _iToranjServ = iToranjServ;
        }

        [HttpPost]
        public IActionResult Galleries([FromBody] Pager request)
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

            var data = _iToranjServ.iGalleryServ.QueryMaker(x => x.Where(y => y.Enabled && y.PortalID == portalId)).OrderBy(x => x.Ordering).Skip(skip).Take(take).Select(x => new {
                x.ID,
                x.Title,
                x.Description,
                x.GalleryType,
                x.Ordering,
                x.PortalID
            }).ToList();
            return Ok(new { 
                BaseAddress = baseAddress,
                Data = data
            });
        }

        [HttpPost]
        public IActionResult GalleryAlbums([FromBody] GalleryAlbumsRequest request)
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

            var data = _iToranjServ.iAlbumServ.QueryMaker(x => x.Where(y => y.Enabled && y.GalleryID == request.GalleryId)).OrderByDescending(x => x.ID).Skip(skip).Take(take).Select(x => new {
                x.ID,
                x.GalleryID,
                x.Title,
                x.Description,
                x.PhotographerName,
                x.Enabled,
                x.Ordering,
                x.Hit,
                x.Logo,
                x.DateOfPhotos
            }).ToList();
            return Ok(new { 
                BaseAdress = baseAddress,
                Data = data
            });
        }

        [HttpPost]
        public async Task<IActionResult> AlbumMedias([FromBody] AlbumMediasRequest request)
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

            var data = await _iToranjServ.iMediaServ.QueryMaker(x => x.Where(y => y.Enabled && y.AlbumID == request.AlbumId)).OrderByDescending(x => x.ID).Skip(skip).Take(take).Select(x => new {
                x.ID,
                x.AlbumID,
                x.Title,
                x.Description,
                x.PhotoType,
                x.Enabled,
                x.Ordering,
                x.Hit,
                x.FileName,
                x.Thumbnail,
                x.Thumbnail2,
                x.BackgroundPlayer,
                x.BackgroundPlayer2
            }).ToListAsync();
            return Ok(new { 
                BaseAdress = baseAddress,
                Data = data
            });
        }

        [HttpPost]
        public async Task<IActionResult> MediaContent([FromBody] MediasRequest request)
        {
            var portalId = Convert.ToInt32(_config.GetSection("ToranjSettings:PortalId").Value);
            var baseAddress = _config.GetSection("ToranjSettings:BaseAddress").Value;

            var media = await _iToranjServ.iMediaServ.FindAsync(x => x.ID == request.MediaId);

            if (media == null)
            {
                return NoContent();
            }

            return Ok(new
            {
                BaseAddress = baseAddress,
                Data = new {
                    media.ID,
                    media.AlbumID,
                    media.Title,
                    media.Description,
                    media.PhotoType,
                    media.Enabled,
                    media.Ordering,
                    media.Hit,
                    media.FileName,
                    media.Thumbnail,
                    media.Thumbnail2,
                    media.BackgroundPlayer,
                    media.BackgroundPlayer2
                }
            });
        }

		[HttpPost]
		public async Task<IActionResult> GetMedias([FromBody] MediaListRequest request) {
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

			int galleryType = 0;
			int galleryId = 0;
			if (request.MediaType == "I") {
				galleryType = 3;
			} else if (request.MediaType == "V") {
				galleryType = 1;
			} else if (request.MediaType == "S") {
				galleryType = 2;
			}

			var gallery = await _iToranjServ.iGalleryServ.FindAsync(x => x.PortalID == portalId && x.GalleryType == galleryType);
			if (gallery != null) {
				galleryId = gallery.ID;
			}
			var albumIds = await _iToranjServ.iAlbumServ.QueryMaker(y => y.Where(x => x.Enabled && x.GalleryID == galleryId)).Select(x => x.ID).ToListAsync();

			var data = await _iToranjServ.iMediaServ.QueryMaker(x => x.Where(y => y.Enabled && y.MediaType == request.MediaType && albumIds.Contains(y.AlbumID))).OrderBy(x => x.Ordering).Skip(skip).Take(take).Select(x => new {
				x.ID,
				x.AlbumID,
				x.Title,
				x.Description,
				x.PhotoType,
				x.Enabled,
				x.Ordering,
				x.Hit,
				x.FileName,
				x.Thumbnail,
				x.Thumbnail2,
				x.BackgroundPlayer,
				x.BackgroundPlayer2
			}).ToListAsync();
			return Ok(new {
				BaseAdress = baseAddress,
				Data = data
			});
		}

	}
}
