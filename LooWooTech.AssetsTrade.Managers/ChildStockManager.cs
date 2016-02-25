using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers
{
    /// <summary>
    /// 子帐户持仓管理
    /// </summary>
    public class ChildStockManager : ManagerBase
    {
        /// <summary>
        /// 同步股票市价
        /// </summary>
        public void SyncPrice()
        {
            var result = Core.ServiceManager.QueryStocks();
            if(!result.Result)
            {
                return;
            }
            

        }
    }
}
