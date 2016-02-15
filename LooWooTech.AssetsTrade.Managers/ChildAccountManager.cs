using LooWooTech.AssetsTrade.Models;
using LooWooTech.AssetsTrade.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers
{
    public class ChildAccountManager : ManagerBase
    {
        public ChildAccount GetAccount(string username,string password = null)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("username");
            }
            using (var db = GetDbContext())
            {
                var entity = db.ChildAccounts.FirstOrDefault(e => e.ChildName.ToLower() == username.ToLower());
                if (entity == null)
                {
                    throw new ArgumentException("用户不存在");
                }
                if (!string.IsNullOrEmpty(password) && entity.Password != password.MD5())
                {
                    throw new ArgumentException("密码不正确");
                }
                return entity;
            }
        }

        public void UpdatePassword(string username,string oldPassword,string newPassword)
        {
            using (var db = GetDbContext())
            {
                var entity = db.ChildAccounts.FirstOrDefault(e => e.ChildName.ToLower() == username.ToLower());
                if (entity == null)
                {
                    throw new ArgumentException("用户不存在");
                }
                if(entity.Password != oldPassword.MD5())
                {
                    throw new ArgumentException("旧密码不正确");
                }
                entity.Password = newPassword.MD5();
                db.SaveChanges();
            }
        }
    }
}
