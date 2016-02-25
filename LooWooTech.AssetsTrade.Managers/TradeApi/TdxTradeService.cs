using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LooWooTech.AssetsTrade.Models;
using System.Runtime.InteropServices;

namespace LooWooTech.AssetsTrade.Managers.TradeApi
{
    public class TdxTradeService : ITradeService
    {
        private static bool _hasLogin = false;

        private int _resultCapacity = 0x1000 * 100;
        private int _errorCapacity = 0x100;

        public ApiResult Login(MainAccount account, ServiceIP ip)
        {
            var success = new ApiResult { Result = true };

            if (_hasLogin) return success;

            TdxTradeApi.SetServer(ip.IPAddress, ip.Port);
            TdxTradeApi.SetAccount(account.MainID, account.TradePassword, account.MessagePassword);

            if (TdxTradeApi.Login())
            {
                _hasLogin = true;
                return success;
            }
            else
            {
                var serverInfo = TdxTradeApi.GetReturnInfo();
                return new ApiResult
                {
                    Result = false,
                    Error = serverInfo
                };
            }
        }

        public void Logout()
        {
            _hasLogin = false;
        }

        public ApiResult Buy(string stockCode, int number, double price)
        {
            var data = new StringBuilder(_resultCapacity);
            var error = new StringBuilder(_errorCapacity);
            var result = TdxTradeApi.ToBuy(stockCode, number, price, data, error) == 1;

            return new ApiResult
            {
                Result = result,
                Data = data.ToString(),
                Error = error.ToString().ToString()
            };
        }

        public ApiResult Sell(string stockCode, int number, double price)
        {
            var data = new StringBuilder(_resultCapacity);
            var error = new StringBuilder(_errorCapacity);
            var result = TdxTradeApi.ToSell(stockCode, number, price, data, error) == 1;

            return new ApiResult
            {
                Result = result,
                Data = data.ToString(),
                Error = error.ToString()
            };
        }

        public ApiResult Cancel(string stockCode, string authorizeIndex)
        {
            var data = new StringBuilder(_resultCapacity);
            var error = new StringBuilder(_errorCapacity);
            TdxTradeApi.CancelOrder(stockCode, authorizeIndex, data, error);
            return new ApiResult
            {
                Result = error.Length == 0,
                Data = data.ToString(),
                Error = error.ToString()
            };
        }

        public ApiResult QueryAuthroizes()
        {
            var data = new StringBuilder(_resultCapacity);
            var error = new StringBuilder(_errorCapacity);
            TdxTradeApi.QueryData(1, data, error);
            return new ApiResult
            {
                Result = error.Length == 0,
                Data = data.ToString(),
                Error = error.ToString()
            };
        }

        public ApiResult QueryHistoryMoney(DateTime startTime, DateTime endTime)
        {
            var data = new StringBuilder(_resultCapacity);
            var error = new StringBuilder(_errorCapacity);
            TdxTradeApi.QueryHistroyMoney(startTime.ToString(), endTime.ToString(), data, error);
            return new ApiResult
            {
                Result = error.Length == 0,
                Data = data.ToString(),
                Error = error.ToString()
            };
        }

        public ApiResult QueryHistoryTrade(DateTime startTime, DateTime endTime)
        {
            var data = new StringBuilder(_resultCapacity);
            var error = new StringBuilder(_errorCapacity);
            TdxTradeApi.QueryHistroyData(startTime.ToString(), endTime.ToString(), data, error);
            return new ApiResult
            {
                Result = error.Length == 0,
                Data = data.ToString(),
                Error = error.ToString()
            };
        }

        public ApiResult QueryMoney()
        {
            var data = new StringBuilder(_resultCapacity);
            var error = new StringBuilder(_errorCapacity);
            TdxTradeApi.QueryData(3, data, error);
            return new ApiResult
            {
                Result = error.Length == 0,
                Data = data.ToString(),
                Error = error.ToString()
            };
        }

        public ApiResult QueryStocks()
        {
            var data = new StringBuilder(_resultCapacity);
            var error = new StringBuilder(_errorCapacity);
            TdxTradeApi.QueryData(0, data, error);
            return new ApiResult
            {
                Result = error.Length == 0,
                Data = data.ToString(),
                Error = error.ToString()
            };
        }

        public ApiResult QueryTrades()
        {
            var data = new StringBuilder(_resultCapacity);
            var error = new StringBuilder(_errorCapacity);
            TdxTradeApi.QueryData(2, data, error);
            return new ApiResult
            {
                Result = error.Length == 0,
                Data = data.ToString(),
                Error = error.ToString()
            };
        }
    }
}
