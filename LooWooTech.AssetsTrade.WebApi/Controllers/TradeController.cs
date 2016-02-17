using LooWooTech.AssetsTrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LooWooTech.AssetsTrade.WebApi.Controllers
{
    public class TradeController : ControllerBase
    {
        private ChildAccount GetCurrentAccount()
        {
            return Core.ChildAccountManager.GetAccount(CurrentUser.ID); 
        }

        /// <summary>
        /// 买入股
        /// </summary>
        public ActionResult Buy(string stockCode, int number, float price)
        {
            var account = GetCurrentAccount();
            Core.TradeManager.Buy(stockCode, number, price, account);
            return View();
        }

        /// <summary>
        /// 卖出股票
        /// </summary>
        public ActionResult Sell(string stockCode,int number,float price)
        {
            var account = GetCurrentAccount();
            Core.TradeManager.Sell(stockCode, number, price);
            return View();
        }
    }
}