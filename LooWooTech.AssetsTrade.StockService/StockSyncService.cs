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
            //Core.ChildStockManager
        }

        protected override int GetInterval()
        {
            var result = 5;
            if (int.TryParse(System.Configuration.ConfigurationManager.AppSettings["SyncInterval"], out result))
            {
                return result;
            }
            return 5;
        }
    }
}
