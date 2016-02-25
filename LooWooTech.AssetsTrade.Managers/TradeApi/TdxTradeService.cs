using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LooWooTech.AssetsTrade.Models;

namespace LooWooTech.AssetsTrade.Managers.TradeApi
{
    public class TdxTradeService : ITradeService
    {
        private static bool _hasLogin = false;

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
            var data = string.Empty;
            var error = string.Empty;
            var result = TdxTradeApi.ToBuy(stockCode, number, price, ref data, ref error) == 1;

            return new ApiResult
            {
                Result = result,
                Data = data,
                Error = error
            };
        }

        public ApiResult Sell(string stockCode, int number, double price)
        {
            var data = string.Empty;
            var error = string.Empty;
            var result = TdxTradeApi.ToSell(stockCode, number, price, ref data, ref error) == 1;

            return new ApiResult
            {
                Result = result,
                Data = data,
                Error = error
            };
        }

        public ApiResult Cancel(string stockCode, string authorizeIndex)
        {
            var data = string.Empty;
            var error = string.Empty;
            TdxTradeApi.CancelOrder(stockCode, authorizeIndex, ref data, ref error);
            return new ApiResult
            {
                Result = string.IsNullOrEmpty(error),
                Data = data,
                Error = error
            };
        }

        public ApiResult QueryAuthroizes()
        {
            var data = string.Empty;
            var error = string.Empty;
            TdxTradeApi.QueryData(1, ref data, ref error);
            return new ApiResult
            {
                Result = string.IsNullOrEmpty(error),
                Data = data,
                Error = error
            };
        }

        public ApiResult QueryHistoryMoney(DateTime startTime, DateTime endTime)
        {
            var data = string.Empty;
            var error = string.Empty;
            TdxTradeApi.QueryHistroyMoney(startTime.ToString(), endTime.ToString(), ref data, ref error);
            return new ApiResult
            {
                Result = string.IsNullOrEmpty(error),
                Data = data,
                Error = error
            };
        }

        public ApiResult QueryHistoryTrade(DateTime startTime, DateTime endTime)
        {
            var data = string.Empty;
            var error = string.Empty;
            TdxTradeApi.QueryHistroyData(startTime.ToString(), endTime.ToString(), ref data, ref error);
            return new ApiResult
            {
                Result = string.IsNullOrEmpty(error),
                Data = data,
                Error = error
            };
        }

        public ApiResult QueryMoney()
        {
            var data = string.Empty;
            var error = string.Empty;
            TdxTradeApi.QueryData(3, ref data, ref error);
            return new ApiResult
            {
                Result = string.IsNullOrEmpty(error),
                Data = data,
                Error = error
            };
        }

        public ApiResult QueryStocks()
        {
            var data = string.Empty;
            var error = string.Empty;
            TdxTradeApi.QueryData(0, ref data, ref error);
            return new ApiResult
            {
                Result = string.IsNullOrEmpty(error),
                Data = data,
                Error = error
            };
        }

        public ApiResult QueryTrades()
        {
            var data = string.Empty;
            var error = string.Empty;
            TdxTradeApi.QueryData(2, ref data, ref error);
            return new ApiResult
            {
                Result = string.IsNullOrEmpty(error),
                Data = data,
                Error = error
            };
        }
    }
}
