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
    public class StockManager : ManagerBase
    {
        /// <summary>
        /// 查询持仓
        /// </summary>
        public List<ChildStock> GetChildStocks(int childId)
        {
            using (var db = GetDbContext())
            {
                return db.ChildStocks.Where(e => e.ChildID == childId).ToList();
            }
        }

        public List<ChildStock> GetMainStocks(string mainId)
        {
            using (var db = GetDbContext())
            {
                var childIds = Core.AccountManager.GetChildIds(mainId);
                return db.ChildStocks.Where(e => childIds.Contains(e.ChildID)).ToList();
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
        /// 查询持仓接口
        /// </summary>
        /// <returns></returns>
        public List<ChildStock> QueryStocks()
        {
            var list = new List<ChildStock>();
            var result = Core.ServiceManager.QueryStocks();
            if (result.Result)
            {
                foreach (var line in result.Data.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    list.Add(ChildStock.Parse(line));
                }
            }
            return list;
        }

        /// <summary>
        /// 同步股票市价
        /// </summary>
        public void SyncStocks()
        {
            var list = QueryStocks();
            if (list.Count == 0) return;

            using (var db = GetDbContext())
            {
                foreach (var item in list)
                {
                    var stock = db.ChildStocks.FirstOrDefault(e => e.StockCode == item.StockCode);
                    if (stock != null)
                    {
                        stock.CurrentPrice = item.CurrentPrice;
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
