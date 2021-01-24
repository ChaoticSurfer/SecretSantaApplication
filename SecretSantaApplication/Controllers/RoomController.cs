using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApplication.Data;
using SecretSantaApplication.Models;

namespace SecretSantaApplication.Controllers
{
    public class RoomController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RoomController(AppDbContext appDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _appDbContext = appDbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public ViewResult Index([Optional] string param)
        {
            var rooms = _appDbContext.Rooms;
            ViewData["message"] = param;
            return View(rooms);
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Create()
        {
            var profileIsCompleted = _appDbContext.Profiles.SingleOrDefault(p =>
                p.EmailAddress == HttpContext.Session.GetString(Helpers.ConstantFields.EmailAddress));
            if (profileIsCompleted == null)
                return RedirectToAction("Profile", "User",
                    new {param = "You have to fill your profile, if you want to create game rooms!"});
            return View();
        }

        [HttpPost]
        [Authorize]
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

                room.Creator = HttpContext.Session.GetString(Helpers.ConstantFields.EmailAddress);
                _appDbContext.Add(room);
                await _appDbContext.SaveChangesAsync();
                return Redirect("/");
            }

            return View();
        }


        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Delete(string roomName)
        {
            Room room = _appDbContext.Rooms.FirstOrDefault(r => r.Name == roomName);
            if (room == null)
            {
                return NotFound();
            }

            string creatorMail = HttpContext.Session.GetString(Helpers.ConstantFields.EmailAddress);
            if (room.Creator != creatorMail)
            {
                return RedirectToAction("Index", "Room", new {param = "Your are not creator of this room!"});
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string path = Path.Combine(wwwRootPath, "images/RoomLogoImages/", room.LogoName);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            _appDbContext.Rooms.Remove(room);
            await _appDbContext.SaveChangesAsync();
            return RedirectToActionPermanent(nameof(Index), "Room");
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Join(string roomName)
        {
            Room room = _appDbContext.Rooms.FirstOrDefault(r => r.Name == roomName);
            if (room == null)
                return new NotFoundResult();

            var mailAddress = HttpContext.Session.GetString(Helpers.ConstantFields.EmailAddress);
            User user = _appDbContext.Users.First(u =>
                u.EmailAddress == mailAddress);

            UserToRoom userToRoom = new UserToRoom();
            userToRoom.Name = room.Name;
            userToRoom.EmailAddress = mailAddress;
            userToRoom.JoinDate = DateTime.Now;
            _appDbContext.Add(userToRoom);
            _appDbContext.SaveChanges();
            return RedirectPermanent("/");
        }
    }
}