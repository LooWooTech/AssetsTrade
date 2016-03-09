using LooWooTech.AssetsTrade.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers
{
    public class ServiceManager : ManagerBase
    {
        private static readonly TradeApi.ServiceInvoker Invoker = new TradeApi.ServiceInvoker();

        public ApiResult Buy(MainAccount account, string stockCode, int number, double price)
        {
            return Invoker.InvokeMethod(account, "Buy", new object[] { stockCode, number, price });
        }

        public ApiResult Sell(MainAccount account, string stockCode, int number, double price)
        {
            return Invoker.InvokeMethod(account, "Sell", new object[] { stockCode, number, price });
        }

        public ApiResult Cancel(MainAccount account, string stockCode, string authorizeIndex)
        {
            return Invoker.InvokeMethod(account, "Cancel", new object[] { stockCode, authorizeIndex });
        }

        public ApiResult QueryStocks(MainAccount account)
        {
            return Invoker.InvokeMethod(account, "QueryStocks", null);
        }

        public ApiResult QueryAuthroizes(MainAccount account)
        {
            return Invoker.InvokeMethod(account, "QueryAuthroizes", null);
        }

        public ApiResult QueryTrades(MainAccount account)
        {
            return Invoker.InvokeMethod(account, "QueryTrades", null);
        }

        public ApiResult QueryMoney(MainAccount account)
        {
            return Invoker.InvokeMethod(account, "QueryMoney", null);
        }

        public ApiResult QueryHistoryTrade(MainAccount account, DateTime startTime, DateTime endTime)
        {
            return Invoker.InvokeMethod(account, "QueryHistoryTrade", new object[] { startTime, endTime });
        }

        public ApiResult QueryHistoryMoney(MainAccount account, DateTime startTime, DateTime endTime)
        {
            return Invoker.InvokeMethod(account, "QueryHistoryMoney", new object[] { startTime, endTime });
        }
    }
}
