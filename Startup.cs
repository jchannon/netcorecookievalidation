using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleApplication
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCookieAuthentication(GetCookieOptions());

            app.MapWhen(x => x.Request.Path == "/", y => y.Run(async (context) =>
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    context.Response.StatusCode = 401; //This will rediret to login route
                    return;
                }
                await context.Response.WriteAsync($"Hello World, you are current logged in as user {context.User.FindFirst(ClaimTypes.Name).Value} aged {context.User.FindFirst("Age").Value}");
            }));

            app.MapWhen(x => x.Request.Path == "/login", y => y.Run(async (context) =>
            {
                //verify user when logging in and get it back
                var user = UserDatabase.GetUser();

                var claims = new List<Claim>(new[]
                {
                        new Claim(ClaimTypes.Name, user.Name),
                        new Claim("Age", user.Age.ToString()),
                });

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "MyCookieMW"));
                await context.Authentication.SignInAsync("MyCookieMW", claimsPrincipal);//Sign user in

                context.Response.Redirect("/");
            }));

            app.MapWhen(x => x.Request.Path == "/changeuser", y => y.Run(context =>
            {
                UserDatabase.ChangeUser("Elliot", 29); //Update user details

                context.Response.Redirect("/");  //Go to root and hope we see our updated info

                return Task.CompletedTask;
            }));
        }

        private CookieAuthenticationOptions GetCookieOptions()
        {
            var options = new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                CookieSecure = CookieSecurePolicy.SameAsRequest,
                AuthenticationScheme = "MyCookieMW",
                CookieHttpOnly = true,
                SlidingExpiration = true,
                CookieName = "MyCookie",
                AutomaticChallenge = true,
                LoginPath = new PathString("/login")
            };

            options.Events = new CookieAuthenticationEvents()
            {
                OnValidatePrincipal = Validator.ValidateAsync
            };

            return options;
        }
    }
}