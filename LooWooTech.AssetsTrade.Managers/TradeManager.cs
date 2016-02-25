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
        private static string CacheKey = "Login";

        /// <summary>
        /// 买入股票
        /// </summary>
        public void ToBuy(string stockCode, int number, double price, int childId)
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

                var main = Core.MainAccountManager.GetServerMainAccount();
                //调用接口购买
                var result = Core.ServiceManager.Buy(stockCode, number, price);
                var authorize = new ChildAuthorize
                {
                    ID = DateTime.Now.ToString("yyyyMMddHHmmssffff"),
                    AuthorizeIndex = "0",
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
                    AuthorizeState = "未报",
                    AuthorizeTime = DateTime.Now,
                    OverFlowMoney = child.UseableMoney - stockPrice,//佣金、过户费要在成交时扣除
                };

                if (result.Result)
                {
                    //委托编号
                    authorize.AuthorizeIndex = result.Data;
                    //扣除余额资金
                    child.UseableMoney -= stockPrice;

                    db.ChildAuthorizes.Add(authorize);
                    db.SaveChanges();
                }
                else
                {
                    authorize.AuthorizeState = "失败";
                    db.ChildAuthorizes.Add(authorize);
                    db.SaveChanges();
                    throw new Exception("委托失败\n" + result.Error);
                }

            }

        }

        /// <summary>
        /// 卖出股票
        /// </summary>
        public void ToSell(string stockCode, int number, double price, int childId)
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
                var main = Core.MainAccountManager.GetServerMainAccount();
                //调用卖出接口 
                var result = Core.ServiceManager.Sell(stockCode, number, price);
                //声明一个新委托
                var authorize = new ChildAuthorize
                {
                    ID = DateTime.Now.ToString("yyyyMMddHHmmssffff"),
                    AuthorizeIndex = "0",
                    AuthorizeCount = number,
                    AuthorizePrice = price,
                    StockCode = stockCode,
                    AuthorizeState = "待报",
                    AuthorizeTime = DateTime.Now,
                    ChildCommission = child.Commission,
                    ClientID = child.ChildID,
                    StockName = stockCode,
                    TradeFlag = "0",
                    OverFlowMoney = child.UseableMoney,
                    MainCommission = main.Commission,
                    MainYinHuaShui = main.YinHuaShui,
                    MainGuoHuFei = main.GuoHuFei,
                };
                //如果调用接口成功
                if(result.Result)
                {
                    //赋值委托编号
                    authorize.AuthorizeIndex = result.Data;
                    //持仓总量-卖出数量
                    stocks.AllCount -= number;
                    //可用数量-卖出数量（可卖出股票必定是可用股票）
                    stocks.UseableCount -= number;

                    db.ChildAuthorizes.Add(authorize);
                    db.SaveChanges();
                }
                else
                {
                    db.ChildAuthorizes.Add(authorize);
                    db.SaveChanges();
                    throw new Exception("卖出失败\n" + result.Error);
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
                var result = Core.ServiceManager.Cancel(stockCode, authorize.AuthorizeIndex);
                if(result.Result)
                {
                    authorize.AuthorizeState = "待撤";
                    db.SaveChanges();
                }
                else
                {
                    throw new Exception("撤单失败\n" + result.Error);
                }
            }
        }
        /// <summary>
        /// 更新委托的状态
        /// </summary>
        public void UpdateAuthorize(ChildAuthorize model)
        {
            using (var db = GetDbContext())
            {
                var entity = db.ChildAuthorizes.FirstOrDefault(e => e.AuthorizeIndex == model.AuthorizeIndex);
                if (entity == null) return;
                var endStatus = "已成,已撤,部撤,废单";
                //如果状态不变且处于终结状态，则说明是当前状态已处理过的委托
                if (entity.AuthorizeState == model.AuthorizeState && endStatus.Contains(model.AuthorizeState))
                {
                    return;
                }

                var isBuy = entity.TradeFlag == "1";
                var child = db.ChildAccounts.FirstOrDefault(e => e.ChildID == entity.ClientID);
                var stock = db.ChildStocks.FirstOrDefault(e => e.StockCode == model.StockCode && e.ChildID == entity.ClientID);
                //本次成交量变化
                if ("已成,部撤".Contains(model.AuthorizeState))
                {
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
                }
                if ("废单,已撤,部撤".Contains(model.AuthorizeState))
                {
                    //如果是撤单
                    if (model.AuthorizeState.Contains("撤"))
                    {
                        //更新撤单数量
                        entity.UndoCount = model.AuthorizeCount - model.StrikeCount;
                    }
                    if (isBuy)
                    {
                        child.UseableMoney += model.StrikePrice * model.StrikeCount;
                        //买入并不会更新股票可用余额 所以只需要更新总量
                        stock.AllCount -= model.StrikeCount;
                    }
                    else
                    {
                        stock.AllCount += model.StrikeCount;
                        stock.UseableCount += model.StrikeCount;
                    }
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
