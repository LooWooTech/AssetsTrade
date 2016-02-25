using LooWooTech.AssetsTrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers
{
    public class MainAccountManager : ManagerBase
    {
        public MainAccount GetModel(string id)
        {
            using (var db = GetDbContext())
            {
                return db.MainAccounts.FirstOrDefault(e => e.MainID == id);
            }
        }

        public List<MainAccount> GetList()
        {
            using (var db = GetDbContext())
            {
                return db.MainAccounts.ToList();
            }
        }

        private static MainAccount _serverAccount;

        public MainAccount GetServerMainAccount()
        {
            if(_serverAccount == null)
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
    }
}
