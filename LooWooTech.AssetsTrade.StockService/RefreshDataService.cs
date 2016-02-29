using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.StockService
{
    internal class RefreshDataService : ServiceBase
    {
        private bool _hasRefreshed;

        protected override void Dowork()
        {
            if (_hasRefreshed)
            {
                if (DateTime.Now.Minute != AppSettings.RefreshTime.Minute)
                {
                    _hasRefreshed = false;
                }
                return;
            }
            if (DateTime.Now.Hour == AppSettings.RefreshTime.Hour && DateTime.Now.Minute == AppSettings.RefreshTime.Minute)
            {
                Core.AccountManager.UpdateChildAccountMoneyAndStocks(AppSettings.MainAccountID);
                _hasRefreshed = true;
            }
        }

        protected override int GetInterval()
        {
            return AppSettings.IsWorkingTime ? 1200 : 30;
        }
    }
}
