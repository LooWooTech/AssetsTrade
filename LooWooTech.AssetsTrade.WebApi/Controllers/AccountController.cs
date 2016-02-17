using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LooWooTech.AssetsTrade.WebApi.Controllers
{
    public class AccountController : ControllerBase
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

        public ActionResult EditPassword(string username, string oldPassword, string newPassword)
        {
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentException("参数不正确");
            }
            if (CurrentUser.Name.ToLower() != username.ToLower())
            {
                throw new HttpException(401, "没有权限修改密码");
            }
            Core.ChildAccountManager.UpdatePassword(username, oldPassword, newPassword);
            return SuccessResult();
        }

        public ActionResult Logout()
        {
            HttpContext.Logout();
            return SuccessResult();
        }
    }
}