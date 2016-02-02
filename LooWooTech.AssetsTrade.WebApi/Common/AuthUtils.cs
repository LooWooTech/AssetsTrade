using LooWooTech.AssetsTrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Collections.Concurrent;
using LooWooTech.AssetsTrade.Common;

namespace LooWooTech.AssetsTrade.WebApi
{
    public static class AuthUtils
    {
        private const string CacheKey = ".user";

        private static LocalCacheService Cache = new LocalCacheService();

        public static string GenerateToken(this HttpContextBase context, User user)
        {
            var ticket = new FormsAuthenticationTicket(user.ID + "|" + user.Username, true, 60);
            var token = FormsAuthentication.Encrypt(ticket);
            UpdateQueryTime(user.ID);
            return token;
        }

        private static void UpdateQueryTime(string userId)
        {
            Cache.HGetOrSet(CacheKey, userId, () => DateTime.Now);
        }

        public static User GetUserID(this HttpContextBase context)
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
                        return new User { ID = userId, Username = username };
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