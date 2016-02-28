using LooWooTech.AssetsTrade.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers
{
    public class ServiceManager : ManagerBase, ITradeService
    {
        private static readonly TradeApi.ServiceInvoker Invoker = new TradeApi.ServiceInvoker();

        public ApiResult Login(MainAccount account, ServiceIP ip)
        {
            return Invoker.InvokeMethod("Login", new object[] { account, ip });
        }

        public ApiResult Buy(string stockCode, int number, double price)
        {
            return Invoker.InvokeMethod("Buy", new object[] { stockCode, number, price });
        }

        public ApiResult Sell(string stockCode, int number, double price)
        {
            return Invoker.InvokeMethod("Sell", new object[] { stockCode, number, price });
        }

        public ApiResult Cancel(string stockCode, string authorizeIndex)
        {
            return Invoker.InvokeMethod("Cancel", new object[] { stockCode, authorizeIndex });
        }

        public ApiResult QueryStocks()
        {
            return Invoker.InvokeMethod("QueryStocks", null);
        }

        public ApiResult QueryAuthroizes()
        {
            return Invoker.InvokeMethod("QueryAuthroizes", null);
        }

        public ApiResult QueryTrades()
        {
            return Invoker.InvokeMethod("QueryTrades", null);
        }

        public ApiResult QueryMoney()
        {
            return Invoker.InvokeMethod("QueryMoney", null);
        }

        public ApiResult QueryHistoryTrade(DateTime startTime, DateTime endTime)
        {
            return Invoker.InvokeMethod("QueryHistoryTrade", new object[] { startTime, endTime });
        }

        public ApiResult QueryHistoryMoney(DateTime startTime, DateTime endTime)
        {
            return Invoker.InvokeMethod("QueryHistoryMoney", new object[] { startTime, endTime });
        }

        public void Logout()
        {
            Invoker.InvokeMethod("Logout", null);
        }
    }
}
