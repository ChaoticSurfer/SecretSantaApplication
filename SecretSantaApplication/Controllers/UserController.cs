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
    public class UserController : Controller
    {
        private readonly Db_AppContext _dbAppContext;

        public UserController(Db_AppContext dbAppContext)
        {
            _dbAppContext = dbAppContext;
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                var checkedUser = _dbAppContext.Users.SingleOrDefault(u => u.EmailAddress == user.EmailAddress);
                if (checkedUser == null)
                {
                    await _dbAppContext.AddAsync(new User
                    {
                        EmailAddress = user.EmailAddress,
                        Password = user.Password
                    });

                    await _dbAppContext.SaveChangesAsync();
                    Utils.Utils.CreateUserIdentity(HttpContext, user.EmailAddress);
                    return RedirectToAction("Profile", "User", new {email = user.EmailAddress});
                }

                ViewData["Message"] = "User with this email address already exists.";
            }

            return View();
        }

        public IActionResult SignIn()
        {
            HttpContext.Session.Remove(Utils.Utils.EmailAddress);
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(User user)
        {
            if (user.EmailAddress != null && user.Password != null)
            {
                var checkEmailAddress = _dbAppContext.Users.FirstOrDefault(u =>
                    u.EmailAddress == user.EmailAddress);

                var checkPassword = _dbAppContext.Users.FirstOrDefault(u =>
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
                    HttpContext.Session.SetString(Utils.Utils.EmailAddress, user.EmailAddress);
                    Utils.Utils.CreateUserIdentity(HttpContext, user.EmailAddress);

                    var checkProfileIsCompleted = _appContext.Profiles.SingleOrDefault(p =>
                        p.EmailAddress == HttpContext.Session.GetString(Utils.Utils.EmailAddress));
                    return checkProfileIsCompleted != null ? RedirectToAction("Index", "Home") : RedirectToAction("Profile", "User");
                }
            }

            return View();
        }

        [Authorize]
        public IActionResult Profile([Optional] string email)
        {
            if (email != null)
                HttpContext.Session.SetString("email", email);
            var profile =
                _dbAppContext.Profiles.SingleOrDefault(p =>
                    p.EmailAddress == HttpContext.Session.GetString(Utils.Utils.EmailAddress));
            if (profile != null)
            {
                ViewData["Email"] = HttpContext.Session.GetString(Utils.Utils.EmailAddress);
                ViewData["birthDate"] = profile.BirthDate;
                ViewData["letterToSecretSanta"] = profile.LetterToSecretSanta;
                return View();
            }

            ViewData["Email"] = HttpContext.Session.GetString(Utils.Utils.EmailAddress);
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Profile(string birthDate, string letterToSecretSanta)
        {
            var profile = _dbAppContext.Profiles.SingleOrDefault(u =>
                u.EmailAddress == HttpContext.Session.GetString(Utils.Utils.EmailAddress));
            if (profile != null)
            {
                _dbAppContext.Remove(profile);
            }

            _dbAppContext.Profiles.Add(new Profile
            {
                EmailAddress = HttpContext.Session.GetString(Utils.Utils.EmailAddress), BirthDate = birthDate,
                LetterToSecretSanta = letterToSecretSanta
            });
            _dbAppContext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}