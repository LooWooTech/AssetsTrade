using LooWooTech.AssetsTrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers
{
    public interface ITradeService
    {
        ApiResult Login(MainAccount account, ApiHost ip);

        void Logout();

        ApiResult Buy(string stockCode, int number, double price);

        ApiResult Sell(string stockCode, int number, double price);

        ApiResult Cancel(string stockCode, string authorizeIndex);

        ApiResult QueryStocks();

        ApiResult QueryAuthroizes();

        ApiResult QueryTrades();

        ApiResult QueryMoney();

        ApiResult QueryHistoryTrade(DateTime startTime, DateTime endTime);

        ApiResult QueryHistoryMoney(DateTime startTime, DateTime endTime);

        //ApiResult QueryQuotes();
    }
}
