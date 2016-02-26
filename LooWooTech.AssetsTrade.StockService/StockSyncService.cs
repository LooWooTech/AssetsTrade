using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.StockService
{
    internal class StockSyncService : ServiceBase
    {
        protected override void Dowork()
        {
            if (ServiceWorkTime.IsWorkingTime)
            {
                Core.ChildStockManager.SyncPrice();
            }
        }

        protected override int GetInterval()
        {
            var result = 0;
            if (!int.TryParse(System.Configuration.ConfigurationManager.AppSettings["SyncInterval"], out result))
            {
                result = 5;
            }
            return ServiceWorkTime.IsWorkingTime ? result : 60;
        }
    }
}
