using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApplication.Models;
using AppContext = SecretSantaApplication.Data.AppContext;

namespace SecretSantaApplication.Controllers
{
    public class SignUpController : Controller
    {
        private readonly AppContext _appContext;

        public SignUpController(AppContext appContext)
        {
            _appContext = appContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(User user, String confirmPassword)
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
    }
}