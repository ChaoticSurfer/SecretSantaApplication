using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApplication.Data;
using SecretSantaApplication.Models;
using SecretSantaApplication.Helpers;

namespace SecretSantaApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _appDbContext;

        public HomeController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString(ConstantFields.EmailAddress) == null)
            {
                return RedirectToAction("SignOut", "User");
            }

            if (_appDbContext.Profiles.SingleOrDefault(p =>
                p.EmailAddress == HttpContext.Session.GetString(ConstantFields.EmailAddress)) == null)
                return RedirectToAction("Profile", "User");

            var secretSanta = _appDbContext.SecretSantas.SingleOrDefault(s =>
                s.Santa == HttpContext.Session.GetString(ConstantFields.EmailAddress));

            var room = _appDbContext.Rooms.SingleOrDefault(room =>
                room.Creator == HttpContext.Session.GetString(ConstantFields.EmailAddress));

            if (room != null && room.IsStarted == false)

                ViewData["IsStarted"] = false;
            else
                ViewData["IsStarted"] = true;
            
            if (secretSanta != null)
            {
                var profile = _appDbContext.Profiles.SingleOrDefault(p => p.EmailAddress == secretSanta.Target);
                if (profile != null)
                    return View(profile);
            }
            ViewData["Message"] = ConstantFields.GameNotStarted;
            return View();
        }

        [Authorize]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}