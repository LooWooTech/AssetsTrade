using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LooWooTech.AssetsTrade.WebApi.Controllers
{
    public class ChildAccountController : ControllerBase
    {
        [UserAuthorize(Disabled = true)]
        public ActionResult Login(string username, string password)
        {
            var account = Core.ChildAccountManager.GetAccount(username, password);
            if (account == null)
            {
                var token = HttpContext.GetAccessToken(account.ChildID.ToString(), account.ChildName);
                return SuccessResult(new
                {
                    token,
                    userId = account.ChildID,
                    username = account.ChildName
                });
            }
            throw new HttpException(401, "用户名或密码不正确");
        }

        public ActionResult Logout()
        {
            HttpContext.Logout();
            return SuccessResult();
        }
    }
}