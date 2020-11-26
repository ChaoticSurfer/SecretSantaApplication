using System;
using System.Linq;
using System.Threading.Tasks;
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
                await _appContext.AddAsync(new User
                {
                    EmailAddress = user.EmailAddress,
                    Password = user.Password,
                });

                await _appContext.SaveChangesAsync();
                Utils.Utils.CreateUserIdentity(HttpContext, user.EmailAddress);
                return RedirectToAction("Index", "Home");
            }

            ViewData["Message"] = "User with this email address already exists.";
            return View();
        }

        public IActionResult SignIn()
        {
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
                    Utils.Utils.CreateUserIdentity(HttpContext, user.EmailAddress);
                    return RedirectToAction("Index", "Home");
                }
            }

            return View();
        }
    }
}