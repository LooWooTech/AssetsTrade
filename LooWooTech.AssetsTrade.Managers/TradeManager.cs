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
        public void Buy(string stockCode, int number, float price, ChildAccount child)
        {
            //验证余额
            var totalPrice = number * 100 * price;
            if (child.UseableMoney < totalPrice)
            {
                throw new Exception("余额不足");
            }
            //验证是否被禁止购买
            using (var db = GetDbContext())
            {
                if (db.StockTradeSets.Any(e => e.StockCode == stockCode))
                {
                    throw new Exception("该股票禁止购买");
                }
            }
            //调用接口购买
            var result = string.Empty;
            var error = string.Empty;
            if (TdxTrade.ToBuy(stockCode, number, price, ref result, ref error) == 1)
            {
                //TODO 根据Result 处理返回的数据
                var code = result;

                using (var db = GetDbContext())
                {
                    //扣除余额资金
                    var childEntity = db.ChildAccounts.FirstOrDefault(e => e.ChildID == child.ChildID);
                    childEntity.UseableMoney -= totalPrice;
                    //
                    var main = db.MainAccounts.FirstOrDefault(e => e.MainID == child.ParentID);
                    //添加委托记录
                    db.ChildAuthorizes.Add(new ChildAuthorize
                    {
                        ID = code,
                        AuthorizeIndex = code,
                        AuthorizeCount = number * 100,
                        AuthorizePrice = price,
                        ClientID = child.ChildID,
                        ChildCommission = child.Commission,
                        StockCode = stockCode,
                        //StockName =  //TODO StockName从何获取？
                        TradeFlag = "1",
                        MainCommission = main.Commission,
                        MainGuoHuFei = main.GuoHuFei,
                        MainYinHuaShui = main.YinHuaShui,
                    });
                    db.SaveChanges();
                }
            }
            else
            {
                throw new Exception(error);
            }
        }


        /// <summary>
        /// 卖出股票
        /// </summary>
        public void Sell(string stockCode, int number, float price)
        {
            //查看持仓 数量是否符合
            //调用卖出接口 
            //发起卖出委托
            throw new NotImplementedException();
        }

        /// <summary>
        /// 撤单
        /// </summary>
        public void CancelOrder(string stockCode,int number)
        {
            //查看委托 是否有足够的数量撤单
            //调用接口
            
            throw new NotImplementedException();
        }
    }
}
