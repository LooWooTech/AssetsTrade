using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LooWooTech.AssetsTrade.WebApi.Controllers
{
    public class QueryController : ControllerBase
    {
        /// <summary>
        /// 查询持仓
        /// </summary>
        public ActionResult Stocks()
        {
            var list = Core.ChildStockManager.GetList(CurrentUser.ID);
            return SuccessResult(list);
        }

        /// <summary>
        /// 查询当日委托
        /// </summary>
        public ActionResult Authorizes()
        {
            var list = Core.ChildAuthorizeManager.GetList(CurrentUser.ID);
            return SuccessResult(list);
        }

        /// <summary>
        /// 查询当日成交
        /// </summary>
        public ActionResult Trades()
        {
            var list = Core.TradeManager.GetTodayTrades(CurrentUser.ID);
            return SuccessResult(list);
        }

        /// <summary>
        /// 查询当日撤单
        /// </summary>
        public ActionResult CancelOrders()
        {
            var list = Core.ChildAuthorizeManager.GetList(CurrentUser.ID);
            return SuccessResult(list.Where(e => e.AuthorizeState.Contains("撤")));
        }

        /// <summary>
        /// 查询历史成交
        /// </summary>
        /// <returns></returns>
        public ActionResult HistoryTrades(DateTime? start, DateTime? end)
        {
            var startDate = start.HasValue ? start.Value : DateTime.Today.AddDays(-1);
            var endDate = end.HasValue ? end.Value : DateTime.Today.AddDays(1);

            var list = Core.TradeManager.GetHistoryTrades(startDate, endDate, CurrentUser.ID);
            return SuccessResult(list);
        }

        /// <summary>
        /// 查询历史资金流水
        /// </summary>
        public ActionResult HistoryMoney(DateTime? start, DateTime? end)
        {
            var startDate = start.HasValue ? start.Value : DateTime.Today.AddDays(-1);
            var endDate = end.HasValue ? end.Value : DateTime.Today.AddDays(1);

            var list = Core.TradeManager.GetHistoryMoney(startDate, endDate, CurrentUser.ID);
            return SuccessResult(list);
        }
    }
}