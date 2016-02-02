using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LooWooTech.AssetsTrade.WebApi
{
    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class UserAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        public bool Disabled { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var user = AuthUtils.GetUserID(httpContext);
            if (user != null)
            {
                httpContext.User = new GenericPrincipal(new GenericIdentity(user.ID), new[] { "user" });
                return true;
            }
            return false;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!Disabled)
            {
                throw new HttpException(401, "请先登录");
            }
        }
    }
}