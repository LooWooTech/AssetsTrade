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
        /// <summary>
        /// 买入股
        /// </summary>
        public ActionResult Buy(string stockCode, int number, double price)
        {
            Core.TradeManager.ToBuy(stockCode, number, price, CurrentUser.ID);
            return SuccessResult();
        }

        /// <summary>
        /// 卖出股票
        /// </summary>
        public ActionResult Sell(string stockCode, int number, double price)
        {
            Core.TradeManager.ToSell(stockCode, number, price, CurrentUser.ID);
            return SuccessResult();
        }

        /// <summary>
        /// 撤单
        /// </summary>
        public ActionResult Cancel(string stockCode, string authorizeIndex)
        {
            Core.TradeManager.CancelOrder(authorizeIndex, stockCode, CurrentUser.ID);
            return SuccessResult();
        }
    }
}