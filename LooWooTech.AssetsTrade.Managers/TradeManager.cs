using LooWooTech.AssetsTrade.Common;
using LooWooTech.AssetsTrade.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers
{
    public class TradeManager : ManagerBase
    {
        static TradeManager()
        {
            try
            {
                var server = System.Configuration.ConfigurationManager.AppSettings["TradeServer"];
                var port = int.Parse(server.Split(':')[1]);
                TdxTrade.SetServer(server, port);
            }
            catch
            {
                throw new Exception("请在Web.config中配置TradeServer 示例：123.45.67.89:1011");
            }
        }

        private static string CacheKey = "Login";

        public bool Login(ChildAccount child)
        {
            var hasLogin = Cache.HGet<DateTime?>(CacheKey, child.ParentID);
            if (hasLogin != null) return true;

            using (var db = GetDbContext())
            {
                var main = db.MainAccounts.FirstOrDefault(e => e.MainID == child.ParentID);
                TdxTrade.SetAccount(main.MainID, main.TradePassword, main.MessagePassword);
                if (TdxTrade.Login())
                {
                    Cache.HSet(CacheKey, child.ParentID, DateTime.Now);
                    return true;
                }
                else
                {
                    throw new Exception("登录失败");
                }
            }
        }

        /// <summary>
        /// 买入股票
        /// </summary>
        public void Buy(string stockCode, int number, float price, int childId)
        {
            using (var db = GetDbContext())
            {
                var child = db.ChildAccounts.FirstOrDefault(e => e.ChildID == childId);
                //股票费用
                var stockPrice = number * price;
                //手续费
                var shouxufei = stockPrice * child.Commission;
                if (child.ISLowFiveMoney == 1 && shouxufei < 5)
                {
                    shouxufei = 5;
                }
                //过户费
                var guohufei = stockCode.StartsWith("6") ? stockPrice * child.GuoHuFei : 0;
                //总费用
                var totalPrice = stockPrice + shouxufei + guohufei;
                //判断余额
                if (child.UseableMoney < totalPrice)
                {
                    throw new Exception("余额不足");
                }
                //验证是否被禁止购买
                if (db.StockTradeSets.Any(e => e.ParentID == child.ParentID && e.StockCode == stockCode))
                {
                    throw new Exception("该股票禁止购买");
                }
                //调用接口购买
                var result = string.Empty;
                var error = string.Empty;
                if (TdxTrade.ToBuy(stockCode, number, price, ref result, ref error) == 1)
                {
                    //TODO 根据Result 处理返回的数据
                    var id = result;
                    //扣除余额资金
                    child.UseableMoney -= stockPrice;
                    var main = db.MainAccounts.FirstOrDefault(e => e.MainID == child.ParentID);
                    //添加委托记录
                    db.ChildAuthorizes.Add(new ChildAuthorize
                    {
                        ID = id,
                        AuthorizeIndex = id,
                        AuthorizeCount = number,
                        AuthorizePrice = price,
                        ClientID = child.ChildID,
                        ChildCommission = child.Commission,
                        StockCode = stockCode,
                        StockName = stockCode,//委托创建时无法获得股票名称
                        TradeFlag = "1",
                        MainCommission = main.Commission,
                        MainGuoHuFei = main.GuoHuFei,
                        MainYinHuaShui = main.YinHuaShui,
                        AuthorizeState = "买入",
                        AuthorizeTime = DateTime.Now,
                        OverFlowMoney = child.UseableMoney - stockPrice,//佣金、过户费要在成交时扣除
                    });
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("买入失败\n" + error);
                }
            }

        }


        /// <summary>
        /// 卖出股票
        /// </summary>
        public void Sell(string stockCode, int number, float price, int childId)
        {
            using (var db = GetDbContext())
            {
                var child = db.ChildAccounts.FirstOrDefault(e => e.ChildID == childId);
                var stocks = db.ChildStocks.FirstOrDefault(e => e.StockCode == stockCode && e.ChildID == child.ChildID);
                //查看持仓 数量是否符合
                if (stocks.UseableCount < number)
                {
                    throw new ArgumentException("没有足够的股票可以卖出");
                }
                //调用卖出接口 
                var result = string.Empty;
                var error = string.Empty;
                if (TdxTrade.ToSell(stockCode, number, price, ref result, ref error) == 1)
                {
                    var parent = db.MainAccounts.FirstOrDefault(e => e.MainID == child.ParentID);
                    //TODO
                    var id = result;
                    //发起卖出委托
                    db.ChildAuthorizes.Add(new ChildAuthorize
                    {
                        ID = id,
                        AuthorizeIndex = id,
                        AuthorizeCount = number,
                        AuthorizePrice = price,
                        StockCode = stockCode,
                        AuthorizeState = "卖出",
                        AuthorizeTime = DateTime.Now,
                        ChildCommission = child.Commission,
                        ClientID = child.ChildID,
                        StockName = stockCode,
                        TradeFlag = "0",
                        OverFlowMoney = child.UseableMoney,
                        MainCommission = parent.Commission,
                        MainYinHuaShui = parent.YinHuaShui,
                        MainGuoHuFei = parent.GuoHuFei,

                    });
                    //TODO
                    stocks.AllCount -= number;
                    stocks.UseableCount -= number;

                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("卖出失败\n" + error);
                }
            }
        }

        /// <summary>
        /// 撤单
        /// </summary>
        public void CancelOrder(string authorizeIndex, string stockCode, int childId)
        {
            using (var db = GetDbContext())
            {
                var authorize = db.ChildAuthorizes.FirstOrDefault(e => e.AuthorizeIndex == authorizeIndex && e.ClientID == childId);
                if (authorize == null)
                {
                    throw new ArgumentException("没有找到该委托");
                }
                if(authorize.AuthorizeState.Contains("撤"))
                {
                    throw new Exception("该委托已提交过撤单");
                }
                var child = db.ChildAccounts.FirstOrDefault(e => e.ChildID == childId);
                //调用撤单接口
                var result = string.Empty;
                var error = string.Empty;
                TdxTrade.CancelOrder(stockCode, authorize.AuthorizeIndex, ref result, ref error);
                if (!string.IsNullOrEmpty(error))
                {
                    authorize.AuthorizeState = "待撤";
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception(error);
                }
            }
        }
    }
}
