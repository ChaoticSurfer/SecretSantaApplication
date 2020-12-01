using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApplication.Models;
using AppContext = SecretSantaApplication.Data.AppContext;

namespace SecretSantaApplication.Controllers
{
    public class UserController : Controller
    {
        private readonly AppContext _appContext;

        public UserController(AppContext appContext)
        {
            _appContext = appContext;
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(User user, String confirmPassword)
        {
            if (user.Password != confirmPassword)
            {
                ViewData["Error"] = "Passwords don't match.";
                return View();
            }

            var checkedUser = _appContext.Users.SingleOrDefault(u => u.EmailAddress == user.EmailAddress);
            if (checkedUser == null)
            {
                Console.WriteLine(confirmPassword);
                await _appContext.AddAsync(new User
                {
                    EmailAddress = user.EmailAddress,
                    Password = user.Password,
                });

                await _appContext.SaveChangesAsync();
                Utils.Utils.CreateUserIdentity(HttpContext, user.EmailAddress);
                return RedirectToAction("Profile", "User", new {email = user.EmailAddress});
            }

            ViewData["Message"] = "User with this email address already exists.";
            return View();
        }

        public IActionResult SignIn()
        {
            HttpContext.Session.Remove(Utils.Utils.EMAIL_ADDRESS);
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(User user)
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
                    HttpContext.Session.SetString(Utils.Utils.EMAIL_ADDRESS, user.EmailAddress);
                    Utils.Utils.CreateUserIdentity(HttpContext, user.EmailAddress);

                    return RedirectToAction("Index", "Home");
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
                _appContext.Profiles.SingleOrDefault(p =>
                    p.EmailAddress == HttpContext.Session.GetString(Utils.Utils.EMAIL_ADDRESS));
            if (profile != null)
            {
                ViewData["Email"] = HttpContext.Session.GetString(Utils.Utils.EMAIL_ADDRESS);
                ViewData["Age"] = profile.Age;
                ViewData["Wishes"] = profile.Wishes;
                return View();
            }

            ViewData["Email"] = HttpContext.Session.GetString(Utils.Utils.EMAIL_ADDRESS);
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Profile(int age, string wishes)
        {
            var profile = _appContext.Profiles.SingleOrDefault(u =>
                u.EmailAddress == HttpContext.Session.GetString(Utils.Utils.EMAIL_ADDRESS));
            if (profile != null)
            {
                _appContext.Remove(profile);
            }
            _appContext.Profiles.Add(new Profile
                {EmailAddress = HttpContext.Session.GetString(Utils.Utils.EMAIL_ADDRESS), Age = age, Wishes = wishes});
            _appContext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
    }
}