using LooWooTech.AssetsTrade.Models;
using LooWooTech.AssetsTrade.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers
{
    public class AccoutManager : ManagerBase
    {
        public ChildAccount GetChildAccount(int childId)
        {
            using (var db = GetDbContext())
            {
                var entity = db.ChildAccounts.FirstOrDefault(e => e.ChildID == childId);
                if (entity != null)
                {
                    entity.Parent = db.MainAccounts.FirstOrDefault(e => e.MainID == entity.ParentID);
                    return entity;
                }
                throw new ArgumentException("不存在该子账户");
            }
        }

        public ChildAccount GetChildAccount(string username, string password = null)
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

        public void UpdateChildAccountPassword(string username, string oldPassword, string newPassword)
        {
            using (var db = GetDbContext())
            {
                var entity = db.ChildAccounts.FirstOrDefault(e => e.ChildName.ToLower() == username.ToLower());
                if (entity == null)
                {
                    throw new ArgumentException("用户不存在");
                }
                if (entity.Password != oldPassword.MD5())
                {
                    throw new ArgumentException("旧密码不正确");
                }
                entity.Password = newPassword.MD5();
                db.SaveChanges();
            }
        }

        public MainAccount GetMainAccount(string mainId)
        {
            using (var db = GetDbContext())
            {
                return db.MainAccounts.FirstOrDefault(e => e.MainID == mainId);
            }
        }

        public int[] GetChildIds(string mainId)
        {
            using (var db = GetDbContext())
            {
                return db.ChildAccounts.Where(e => e.ParentID == mainId).Select(e => e.ChildID).ToArray();
            }
        }

        private static MainAccount _serverAccount;

        public MainAccount GetServerMainAccount()
        {
            if (_serverAccount == null)
            {
                var mainAccountId = System.Configuration.ConfigurationManager.AppSettings["MainAccountID"];
                using (var db = GetDbContext())
                {
                    _serverAccount = db.MainAccounts.FirstOrDefault(e => e.MainID == mainAccountId);
                    if (_serverAccount == null)
                    {
                        throw new Exception("请在配置文件中填写主账户MainAccountID的值");
                    }
                }
            }

            return _serverAccount;
        }

        public void UpdateChildAccountMoneyAndStocks(string mainId)
        {
            using (var db = GetDbContext())
            {
                //更新所有子帐户的可取余额
                var childIds = GetChildIds(mainId);
                foreach (var childId in childIds)
                {
                    var child = db.ChildAccounts.FirstOrDefault(e => e.ChildID == childId);
                    child.AdvisableMoney = child.UseableMoney;
                }
                //更新所有子帐户的持仓余额
                var stocks = db.ChildStocks.Where(e => childIds.Contains(e.ChildID));
                foreach(var stock in stocks)
                {
                    stock.UseableCount = stock.AllCount;
                    //如果持仓为0，则删除该股票
                    if(stock.UseableCount == 0)
                    {
                        db.ChildStocks.Remove(stock);
                    }
                }

                db.SaveChanges();
            }
        }

    }
}
