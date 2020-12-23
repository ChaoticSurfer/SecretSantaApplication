using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace SecretSantaApplication.Services
{
    public class Identity
    {
        public void CreateUserIdentity(HttpContext httpContext, String emailAddress)
        {
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Email, emailAddress)
            };

            var userIdentity = new ClaimsIdentity(userClaims, "authentication");
            var userPrincipal = new ClaimsPrincipal(new[] {userIdentity});

            httpContext.SignInAsync(userPrincipal);
        }
    }
}