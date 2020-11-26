using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApplication.Data;
using SecretSantaApplication.Models;

namespace SecretSantaApplication.Controllers
{
    public class SignInController : Controller
    {
        private readonly AppContext _appContext;

        public SignInController(AppContext appContext)
        {
            _appContext = appContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(User user)
        {
            if (user.EmailAddress != null && user.Password != null)
            {
                var checkEmailAddress = _appContext.Users.FirstOrDefault(u =>
                    u.EmailAddress == user.EmailAddress);

                var checkPassword = _appContext.Users.FirstOrDefault(u =>
                    u.Password == user.Password);

                if (checkEmailAddress == null)
                {
                    ViewData["Message"] = "User does not exists.";
                }
                else if (checkPassword == null)
                {
                    ViewData["Message"] = "Password is incorrect.";
                }
                else
                {
                    Utils.Utils.CreateUserIdentity(HttpContext, user.EmailAddress);
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }
    }
}