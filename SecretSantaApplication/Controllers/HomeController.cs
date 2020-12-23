using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApplication.Data;
using SecretSantaApplication.Models;

namespace SecretSantaApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly Db_AppContext _appContext;

        public HomeController(Db_AppContext appContext)
        {
            _appContext = appContext;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString(Utils.Utils.EmailAddress) == null)
            {
                return RedirectToAction("SignOut", "Home");
            }

            if (_appContext.Profiles.SingleOrDefault(p =>
                p.EmailAddress == HttpContext.Session.GetString(Utils.Utils.EmailAddress)) == null)
                return RedirectToAction("Profile", "User");

            var secretSanta = _appContext.SecretSantas.SingleOrDefault(s =>
                s.Santa == HttpContext.Session.GetString(Utils.Utils.EmailAddress));
            if (secretSanta != null)
            {
                var profile = _appContext.Profiles.SingleOrDefault(p => p.EmailAddress == secretSanta.Target);
                if (profile != null)
                    return View(profile);
            }

            ViewData["Message"] = Utils.Utils.GameNotStarted;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> SignOut() // should be part of User
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        //------
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}