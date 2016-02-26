using LooWooTech.AssetsTrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers
{
    /// <summary>
    /// 子帐户持仓管理
    /// </summary>
    public class ChildStockManager : ManagerBase
    {
        /// <summary>
        /// 查询持仓
        /// </summary>
        public List<ChildStock> GetList(int childId)
        {
            using (var db = GetDbContext())
            {
                return db.ChildStocks.Where(e => e.ChildID == childId).ToList();
            }
        }

        public string[] GetStockCodes(int childId)
        {
            using (var db = GetDbContext())
            {
                return db.ChildStocks.Where(e => e.ChildID == childId).Select(e => e.StockCode).ToArray();
            }
        }

        /// <summary>
        /// 同步股票市价
        /// </summary>
        public void SyncPrice()
        {
            var result = Core.ServiceManager.QueryStocks();
            if (!result.Result)
            {
                return;
            }
            using (var db = GetDbContext())
            {
                //0         1       2       3       4       5       6       7         8      9           10  11
                //"600118\t中国卫星\t100.00\t100.00\t86.179\t30.960\t3096.00\t-5530.06\t-64.17\tA474859797\t1\t1\t\t"
                foreach (var line in result.Data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    var fields = line.Split('\t');
                    var stockCode = fields[0];
                    var price = double.Parse(fields[6]);
                    var stock = db.ChildStocks.FirstOrDefault(e => e.StockCode == stockCode);
                    if (stock != null)
                    {
                        stock.CurrentPrice = price;
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
