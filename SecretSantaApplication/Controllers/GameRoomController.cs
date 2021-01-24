using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApplication.Data;
using SecretSantaApplication.Extensions;
using SecretSantaApplication.Helpers;
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
            ViewData["Message"] = param;
            return View(rooms);
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult> Create([Optional] string param)
        {
            var profileIsCompleted = _appDbContext.Profiles.SingleOrDefault(p =>
                p.EmailAddress == HttpContext.Session.GetString(Helpers.ConstantFields.EmailAddress));
            if (profileIsCompleted == null)
                return RedirectToAction("Profile", "User",
                    new {param = "You have to fill your profile, if you want to create game rooms!"});
            ViewData["Message"] = param;
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Create(Room gameRoom)
        {
            var room = _appDbContext.Rooms.SingleOrDefault(r =>
                r.Creator == HttpContext.Session.GetString(ConstantFields.EmailAddress));

            if (room != null)
            {
                return RedirectToAction("Create", "Room",
                    new {param = "You have already created room,you only can create one room!"});
            }

            if (ModelState.IsValid)
            {
                // save image to wwwwroot/images
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(gameRoom.ImageLogoFile.FileName);
                string extension = Path.GetExtension(gameRoom.ImageLogoFile.FileName);
                gameRoom.LogoName = fileName = fileName + DateTime.Now.ToString("yyyy-MM-dd__ss-ffffff") + extension;
                string path = Path.Combine(wwwRootPath + "/images/RoomLogoImages/" + fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await gameRoom.ImageLogoFile.CopyToAsync(fileStream);
                }

                gameRoom.Creator = HttpContext.Session.GetString(ConstantFields.EmailAddress);
                _appDbContext.Add(gameRoom);
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

            string creatorMail = HttpContext.Session.GetString(ConstantFields.EmailAddress);
            if (room.Creator != creatorMail)
            {
                return RedirectToAction("Index", "Room", new {param = "Your are not creator of this room!"});
            }

            if (room.IsStarted)
            {
                return RedirectToAction("Index", "Room",
                    new {param = "The game has already started in this game room!"});
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
            var room = _appDbContext.Rooms.FirstOrDefault(r => r.Name == roomName);
            if (room == null)
                return new NotFoundResult();

            var profile = _appDbContext.Profiles.FirstOrDefault(p =>
                p.EmailAddress == HttpContext.Session.GetString(ConstantFields.EmailAddress));

            if (profile == null)
            {
                return RedirectToAction("Index", "Room",
                    new {param = "You have to finish filling profile to join in game room!"});
            }

            if (room.IsStarted)
            {
                return RedirectToAction("Index", "Room",
                    new {param = "You have missed, game in this room has already started!"});
            }

            var userInRoom = _appDbContext.UserToRooms.SingleOrDefault(u =>
                u.EmailAddress == HttpContext.Session.GetString(ConstantFields.EmailAddress));

            if (userInRoom != null)
            {
                return RedirectToAction("Index", "Room",
                    new {param = "You have already joined in some room, you can join in only one room!"});
            }

            var mailAddress = HttpContext.Session.GetString(ConstantFields.EmailAddress);
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

        [Authorize]
        public IActionResult Start()
        {
            var room = _appDbContext.Rooms.SingleOrDefault(r =>
                r.Creator == HttpContext.Session.GetString(ConstantFields.EmailAddress));
            var users = _appDbContext.UserToRooms.Where(r => r.Name == room.Name).ToList();
            var santaTargets = GetSantaTargets(users);
            foreach (var pairs in santaTargets)
            {
                _appDbContext.Add(new SecretSanta
                {
                    Santa = pairs.Item1.EmailAddress,
                    Target = pairs.Item2.EmailAddress
                });
                _appDbContext.SaveChanges();
            }

            room.IsStarted = true;
            _appDbContext.Rooms.Update(room);
            _appDbContext.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        private static IEnumerable<(T, T)> GetSantaTargets<T>(List<T> players)
        {
            var targets = new List<(T, T)>();
            var shuffledPlayers = players.Shuffle().ToList();
            for (int i = 0; i < shuffledPlayers.Count; i++)
            {
                if (i == shuffledPlayers.Count - 1)
                {
                    targets.Add((shuffledPlayers[i], shuffledPlayers[0]));
                    break;
                }

                targets.Add((shuffledPlayers[i], shuffledPlayers[i + 1]));
            }

            return targets;
        }
    }
}