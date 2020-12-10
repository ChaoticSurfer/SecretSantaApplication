using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using AppContext = SecretSantaApplication.Data.AppContext;
using Microsoft.AspNetCore.Mvc;


namespace SecretSantaApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppContext _appContext;

        public HomeController(AppContext appContext)
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

        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index");
        }
    }
}