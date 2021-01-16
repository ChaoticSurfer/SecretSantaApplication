using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApplication.Data;
using SecretSantaApplication.Helpers;
using SecretSantaApplication.Models;
using SecretSantaApplication.Services;

namespace SecretSantaApplication.Controllers
{
    public class UserController : Controller
    {
        private readonly AppDbContext _appDbContext;
        private readonly PasswordSecurity _passwordSecurity;
        private readonly Identity _identity;

        public UserController(AppDbContext appDbContext, PasswordSecurity passwordSecurity, Identity identity)
        {
            _appDbContext = appDbContext;
            _passwordSecurity = passwordSecurity;
            _identity = identity;
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
                var checkedUser = _appDbContext.Users.SingleOrDefault(u => u.EmailAddress == user.EmailAddress);
                if (checkedUser == null)
                {
                    await _appDbContext.AddAsync(new User
                    {
                        EmailAddress = user.EmailAddress,
                        Password = _passwordSecurity.Encrypt(user.Password)
                    });

                    await _appDbContext.SaveChangesAsync();
                    _identity.CreateUserIdentity(HttpContext, user.EmailAddress);
                    return RedirectToAction("Profile", "User", new {email = user.EmailAddress});
                }

                ViewData["Message"] = "User with this email address already exists.";
            }

            return View();
        }

        public IActionResult SignIn()
        {
            HttpContext.Session.Remove(ConstantFields.EmailAddress);
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(User user)
        {
            if (user.EmailAddress != null && user.Password != null)
            {
                var checkEmailAddress = _appDbContext.Users.FirstOrDefault(u =>
                    u.EmailAddress == user.EmailAddress);

                var checkPassword = _appDbContext.Users.FirstOrDefault(u =>
                    u.Password == _passwordSecurity.Encrypt(user.Password));
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
                    HttpContext.Session.SetString(ConstantFields.EmailAddress, user.EmailAddress);
                    _identity.CreateUserIdentity(HttpContext, user.EmailAddress);

                    var checkProfileIsCompleted = _appDbContext.Profiles.SingleOrDefault(p =>
                        p.EmailAddress == HttpContext.Session.GetString(ConstantFields.EmailAddress));
                    return checkProfileIsCompleted != null
                        ? RedirectToAction("Index", "Home")
                        : RedirectToAction("Profile", "User");
                }
            }

            return View();
        }

        [Authorize]
        public IActionResult Profile([Optional] string email, [Optional] string message, [Optional] string otherParams)
        {
            if (email != null)
                HttpContext.Session.SetString("email", email);
            var profile =
                _appDbContext.Profiles.SingleOrDefault(p =>
                    p.EmailAddress == HttpContext.Session.GetString(ConstantFields.EmailAddress));
            if (profile != null)
            {
                ViewData["Email"] = HttpContext.Session.GetString(ConstantFields.EmailAddress);
                ViewData["birthDate"] = profile.BirthDate;
                ViewData["letterToSecretSanta"] = profile.LetterToSecretSanta;
                return View();
            }

            ViewData["Email"] = HttpContext.Session.GetString(ConstantFields.EmailAddress);
            ViewData["Message"] = message;
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Profile(string birthDate, string letterToSecretSanta)
        {
            var profile = _appDbContext.Profiles.SingleOrDefault(u =>
                u.EmailAddress == HttpContext.Session.GetString(ConstantFields.EmailAddress));
            if (profile != null)
            {
                _appDbContext.Remove(profile);
            }

            _appDbContext.Profiles.Add(new Profile
            {
                EmailAddress = HttpContext.Session.GetString(ConstantFields.EmailAddress), BirthDate = birthDate,
                LetterToSecretSanta = letterToSecretSanta
            });
            _appDbContext.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        public new async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}