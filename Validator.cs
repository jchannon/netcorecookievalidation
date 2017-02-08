using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace ConsoleApplication
{
    public static class Validator
    {
        public static async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            Console.WriteLine($"VALIDATING {context.Request.Path}");

            //Get user per request
            var user = UserDatabase.GetUser();

            //User has been deleted in the back end so invalidate the cookie
            if (user == null)
            {
                context.RejectPrincipal();
                await context.HttpContext.Authentication.SignOutAsync("MyCookieMW");
                return;
            }

            var claims = new List<Claim>(new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("Age", user.Age.ToString()),
            });

            //Backend user has changed details and differs from cookie so update cookie
            if (!context.Principal.Claims.Select(x => x.Value).SequenceEqual(claims.Select(y => y.Value)))
            {
                Console.WriteLine("UPDATE COOKIE");

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "MyCookieMW"));

                context.ReplacePrincipal(claimsPrincipal);
                context.ShouldRenew = true;
            }
        }
    }
}