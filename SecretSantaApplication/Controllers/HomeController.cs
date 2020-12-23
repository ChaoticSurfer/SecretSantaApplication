using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApplication.Data;
using SecretSantaApplication.Models;

namespace SecretSantaApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly Db_AppContext _dbAppContext;

        public HomeController(Db_AppContext dbAppContext)
        {
            _dbAppContext = dbAppContext;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString(Utils.Utils.EmailAddress) == null)
            {
                return RedirectToAction("SignOut", "User");
            }

            if (_dbAppContext.Profiles.SingleOrDefault(p =>
                p.EmailAddress == HttpContext.Session.GetString(Utils.Utils.EmailAddress)) == null)
                return RedirectToAction("Profile", "User");

            var secretSanta = _dbAppContext.SecretSantas.SingleOrDefault(s =>
                s.Santa == HttpContext.Session.GetString(Utils.Utils.EmailAddress));
            if (secretSanta != null)
            {
                var profile = _dbAppContext.Profiles.SingleOrDefault(p => p.EmailAddress == secretSanta.Target);
                if (profile != null)
                    return View(profile);
            }

            ViewData["Message"] = Utils.Utils.GameNotStarted;
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