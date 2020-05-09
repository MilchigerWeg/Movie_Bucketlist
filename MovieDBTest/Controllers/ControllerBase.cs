using Microsoft.AspNetCore.Http;
using MovieDBTest.Models;
using System;
using Microsoft.AspNetCore.Mvc;

namespace MovieDBTest.Controllers
{
    public class ControllerBase : Controller
    {
        /// <summary>
        /// setzt session cookie für 10 min
        /// </summary>
        /// <param name="user"></param>
        protected void SetUserCookie(string user)
        {
            var option = new CookieOptions();
            option.Expires = DateTime.Now.AddMinutes(10);
            Response.Cookies.Append("moviedbsiteusername", user, option);
        }

        /// <summary>
        /// holt user zu session aus cookie
        /// </summary>
        /// <returns>User zu Session</returns>
        protected User GetSignedInUser()
        {
            var userName = Request.Cookies["moviedbsiteusername"];
            var provider = new Provider.MongoDbProvider();

            return provider.GetUser(userName);
        }

        /// <summary>
        /// Refreshed die Session, wenn eine valide besteht
        /// </summary>
        protected void RefreshCookieSession()
        {
            var user = GetSignedInUser();

            if (user != null)
            {
                SetUserCookie(user.Name);
            }
        }

        /// <summary>
        /// Meldet Nutzer ab
        /// </summary>
        protected void SignOutFromSession()
        {
            var option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Append("moviedbsiteusername", "", option);

        }
    }
}
