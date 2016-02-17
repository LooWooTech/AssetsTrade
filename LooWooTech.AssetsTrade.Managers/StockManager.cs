using LooWooTech.AssetsTrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers
{
    public class StockManager : ManagerBase
    {
        public void Buy(string stockCode, int number, float price, ChildAccount account)
        {
            //验证余额
            var totalPrice = number * 100 * price;
            if (account.UseableMoney < totalPrice)
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

            //添加委托记录
            using (var db = GetDbContext())
            {
                db.ChildAuthorizes.Add(new ChildAuthorize
                {
                    AuthorizeCount = number * 100,
                    AuthorizePrice = price,
                });
            }
        }
    }
}
