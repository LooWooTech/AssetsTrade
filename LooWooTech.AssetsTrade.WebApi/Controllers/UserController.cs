using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LooWooTech.AssetsTrade.WebApi.Controllers
{
    public class UserController : ControllerBase
    {
        [UserAuthorize(Disabled = true)]
        public ActionResult Login(string username, string password)
        {
            var user = Core.UserManager.GetUser(username, password);
            if (user == null)
            {
                var token = HttpContext.GenerateToken(user);
                return SuccessResult(new
                {
                    token,
                    userId = user.ID,
                    username = user.Username
                });
            }
            throw new HttpException(401, "用户名或密码不正确");
        }
    }
}