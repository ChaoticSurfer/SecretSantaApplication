using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SecretSantaApplication.Data;
using SecretSantaApplication.Models;

namespace SecretSantaApplication.Controllers
{
    public class RoomController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly Db_AppContext _dbAppContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RoomController(ILogger<HomeController> logger, Db_AppContext dbAppContext,
            IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _dbAppContext = dbAppContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public ViewResult List()
        {
            var rooms = _dbAppContext.Rooms;
            return View(rooms);
        }


        [HttpGet]
        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Room room)
        {
            if (ModelState.IsValid)
            {
                // save image to wwwwroot/images
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(room.ImageLogoFile.FileName);
                string extension = Path.GetExtension(room.ImageLogoFile.FileName);
                room.LogoName = fileName = fileName + DateTime.Now.ToString("yyyy-MM-dd__ss-ffffff") + extension;
                string path = Path.Combine(wwwRootPath + "/images/RoomLogoImages/" + fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await room.ImageLogoFile.CopyToAsync(fileStream);
                }

                room.Creator = "creator";
                _dbAppContext.Add(room);
                await _dbAppContext.SaveChangesAsync();
                return Redirect("/");
            }

            return View();
        }


        [Authorize]
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> Delete(string id)
        {
            Room room = _dbAppContext.Rooms.FirstOrDefault(r => r.Name == id);
            if (room == null)
            {
                return NotFound();
            }

            string creatorMail = HttpContext.Session.GetString(Utils.Utils.EMAIL_ADDRESS);
            if (room.Creator != creatorMail)
            {
                return Unauthorized();
            }
            
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string path = Path.Combine(wwwRootPath, "images/RoomLogoImages/", room.LogoName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            _dbAppContext.Rooms.Remove(room);
            await _dbAppContext.SaveChangesAsync();
            return RedirectToActionPermanent(nameof(List), "Room");
        }
    }
}