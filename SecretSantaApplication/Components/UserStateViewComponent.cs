using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SecretSantaApplication.Helpers;
using SecretSantaApplication.Models;


namespace SecretSantaApplication.Components
{
    public class UserStateViewComponent : ViewComponent
    {
        private HttpContext _httpContext;
        private bool isLoggedIn;

        UserStateViewComponent(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public IViewComponentResult Invoke()
        {
            isLoggedIn = _httpContext.Session.Keys.Contains(ConstantFields.EmailAddress);
            return View(isLoggedIn);
        }
    }
}