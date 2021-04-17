using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Startapp.Server.Helpers;
using Startapp.Shared;
using Startapp.Shared.Core;
using Startapp.Shared.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Startapp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class PictureController : ControllerBase
    {
        private readonly IDataStore _dataStore; 
        private readonly IAccountManager _accountManager;

        public PictureController(IDataStore dataStore, IAccountManager accountManager)
        {
            _dataStore = dataStore;
            _accountManager = accountManager;
        }

        [Route("upload/{id}/{table}")]
        [HttpPost]
        public async Task<IActionResult> Upload(string id= null, string table=null)
        {
            //var result = await _dataStore.GetArticleAsync(id);
            //List<int> img_ids = new List<int>();
            string filePath = string.Empty;
            var Picture = Request.Form.Files[0];
            if (table.ToLower() == "user")
            {
               filePath = await AddPictureForProfile(Picture, id);
            }
            if (table.ToLower() == "article")
            {
               filePath = await AddPictureForArticle(Picture, int.Parse(id));
            }           
            
            return Ok(filePath);
        }
             
       
        private async Task<string> AddPictureForArticle(IFormFile picture, int id)
        {
            string Id = string.Empty;
            var result = await _dataStore.GetArticleDetailsAsync(id);
            if (result.Succeeded)
            {
                Picture pic = new Picture
                {
                    Created = DateTime.Now.Date,
                    Article = result.Article,
                    Extension = ".jpg"
                };
                Guid picId = await _dataStore.AddPictureAsync(pic);
                var path =  PicExtensions.Compress(picture,"n", 640, picId + ".jpg"); //you can use this: Path.GetExtension(picture.FileName)
                PicExtensions.Compress(picture,"m", 360, picId + ".jpg"); // to save as orgine, for me i prefere use .jpg to optimise resources
                PicExtensions.Compress(picture,"s", 160, picId + ".jpg"); // because Png or bitmap consume more resources
                Id = picId.ToString();
            }
            return Id;
        }
        private async Task<string> AddPictureForProfile(IFormFile picture, string userId)
        {
            //string userId = Utilities.GetUserId(this.User);
            string filePath = string.Empty;
            var user = await _accountManager.GetUserByIdAsync(userId);
            if (user != null)
            {
                if (! string.IsNullOrEmpty(user.Photo))
                {
                    PictureExtensions.DeleteFile("i", user.Photo);
                }
                user.Photo = Guid.NewGuid() + Path.GetExtension(picture.FileName);
                await _accountManager.UpdateUserAsync(user);
                filePath = PicExtensions.Compress(picture,"i", 120, user.Photo);
            }
            return filePath;
        }

        [HttpDelete("pictures/{id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<Picture>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var pic = await _dataStore.DeletePictureAsync(Guid.Parse(id));
            if (!string.IsNullOrEmpty(pic.Id.ToString()))
            {
                PictureExtensions.DeleteFile("n", pic.Id + pic.Extension);
                PictureExtensions.DeleteFile("m", pic.Id + pic.Extension);
                PictureExtensions.DeleteFile("s", pic.Id + pic.Extension);

                var json = new JsonResponse
                {
                    Json = pic
                };
                return Ok(json);
            }
            return BadRequest();
        }


    }
}
