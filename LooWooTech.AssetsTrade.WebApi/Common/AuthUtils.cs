using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Collections.Concurrent;
using LooWooTech.AssetsTrade.Common;
using System.Threading;

namespace LooWooTech.AssetsTrade.WebApi
{
    public static class AuthUtils
    {
        private const string CacheKey = ".user";

        private static LocalCacheService Cache = new LocalCacheService();

        public static string GetAccessToken(this HttpContextBase context, string userId,string username)
        {
            var ticket = new FormsAuthenticationTicket(userId + "|" + username, true, 60);
            var token = FormsAuthentication.Encrypt(ticket);
            UpdateQueryTime(userId);
            return token;
        }

        public static void Logout(this HttpContextBase context)
        {
            var userId = context.User.Identity.Name;
            if (!string.IsNullOrEmpty(userId))
            {
                Cache.HRemove(CacheKey, userId);
            }
        }

        private static void UpdateQueryTime(string userId)
        {
            Cache.HGetOrSet(CacheKey, userId, () => DateTime.Now);
        }

        public static UserIdentity GetCurrentUser(this HttpContextBase context)
        {
            var token = context.Request.Headers["token"];
            if (string.IsNullOrEmpty(token)) return null;

            try
            {
                var ticket = FormsAuthentication.Decrypt(token);
                if (ticket != null && !string.IsNullOrEmpty(ticket.Name))
                {
                    var values = ticket.Name.Split('|');
                    if (values.Length == 2)
                    {
                        var userId = values[0];
                        var username = values[1];
                        var lastQueryTime = Cache.HGet<DateTime>(CacheKey, userId);
                        //操作时间过期
                        if ((DateTime.Now - lastQueryTime).TotalMinutes > 20)
                        {
                            Cache.HRemove(CacheKey, userId);
                            return null;
                        }
                        else
                        {
                            UpdateQueryTime(userId);
                        }
                        return new UserIdentity { ID = userId, Name = username };
                    }
                }
            }
            catch
            {
            }
            return null;
        }
    }
}