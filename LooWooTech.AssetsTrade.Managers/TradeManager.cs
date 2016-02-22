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
        public void ToBuy(string stockCode, int number, float price, int childId)
        {
            using (var db = GetDbContext())
            {
                var child = db.ChildAccounts.FirstOrDefault(e => e.ChildID == childId);
                //股票费用
                var stockPrice = number * price;

                //总费用
                var totalPrice = stockPrice + child.GetShouXuFei(stockCode, price, number) + child.GetGuoHuFei(stockCode, price, number);
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
        public void ToSell(string stockCode, int number, float price, int childId)
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
                if (authorize.AuthorizeState.Contains("撤"))
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

        public void UpdateAuthorize(ChildAuthorize model)
        {
            using (var db = GetDbContext())
            {
                var entity = db.ChildAuthorizes.FirstOrDefault(e => e.AuthorizeIndex == model.AuthorizeIndex);
                if (entity == null) return;
                var endStatus = "已成,已撤,部撤,已报";
                //如果状态不变且处于终结状态，则说明是当前状态已处理过的委托
                if (entity.AuthorizeState == model.AuthorizeState && endStatus.Contains(model.AuthorizeState))
                {
                    return;
                }

                var isBuy = entity.TradeFlag == "1";
                var child = db.ChildAccounts.FirstOrDefault(e => e.ChildID == entity.ClientID);
                var stock = db.ChildStocks.FirstOrDefault(e => e.StockCode == model.StockCode && e.ChildID == entity.ClientID);
                //本次成交量变化
                switch (model.AuthorizeState)
                {
                    case "已成":
                    case "已撤":
                    case "部撤":
                        var shouxufei = child.GetShouXuFei(model.StockCode, model.StrikePrice, model.StrikeCount);
                        var guohufei = child.GetGuoHuFei(model.StockCode, model.StrikePrice, model.StrikeCount);
                        //扣除手续费和过户费
                        child.UseableMoney = child.UseableMoney - shouxufei - guohufei;
                        if (isBuy)
                        {
                            //如果是买入，持仓股票总量增加
                            if (stock != null)
                            {
                                stock.AllCount += model.StrikeCount;
                            }
                            else
                            {
                                //如果持仓不存在该股，则添加新纪录
                                stock = new ChildStock
                                {
                                    AllCount = model.StrikeCount,
                                    ChildID = entity.ClientID,
                                    CurrentPrice = model.StrikePrice,
                                    LastTime = DateTime.Now.ToUnixTime(),
                                    PrimeCost = model.StrikePrice,
                                    StockCode = model.StockCode,
                                    StockName = model.StockName,
                                    ID = DateTime.Now.ToString("yyyyMMddHHmmssffff")
                                };
                                db.ChildStocks.Add(stock);
                            }
                        }
                        else
                        {
                            //如果是卖出，扣除印花税，另外余额增加股票的市值
                            child.UseableMoney -= child.GetYinHuaShui(model.StockCode, model.StrikePrice, model.StrikeCount);
                            child.UseableMoney += model.StrikePrice * model.StrikeCount;

                            //更新持仓数量
                            stock.AllCount -= model.StrikeCount;
                            stock.UseableCount -= model.StrikeCount;
                        }

                        //如果是撤单
                        if (model.AuthorizeState.Contains("撤"))
                        {
                            //更新撤单数量
                            entity.UndoCount = model.AuthorizeCount - model.StrikeCount;
                            if (isBuy)
                            {
                                child.UseableMoney += model.StrikePrice * model.StrikeCount;
                                stock.AllCount -= model.StrikeCount;
                                stock.UseableCount -= model.StrikeCount;
                            }
                            else
                            {
                                stock.AllCount += model.StrikeCount;
                                stock.UseableCount += model.StrikeCount;
                            }
                        }
                        //成交时间
                        entity.TradeTime = DateTime.Now.ToUnixTime();

                        break;
                    default:
                        break;
                }
                entity.StrikeCount = model.StrikeCount;
                entity.StrikePrice = model.StrikePrice;
                entity.AuthorizeState = model.AuthorizeState;
                entity.StockName = model.StockName;

                db.SaveChanges();
            }
        }
    }
}
