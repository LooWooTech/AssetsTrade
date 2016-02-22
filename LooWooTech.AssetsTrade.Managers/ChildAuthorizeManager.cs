using LooWooTech.AssetsTrade.Common;
using LooWooTech.AssetsTrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers
{
    public class ChildAuthorizeManager : ManagerBase
    {
        /// <summary>
        /// 通过接口获取“当日委托”列表
        /// </summary>
        public List<ChildAuthorize> GetTodayAuthorize()
        {
            var result = string.Empty;
            var error = string.Empty;
            TdxTrade.QueryData(1, ref result, ref error);
            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception(error);
            }

            var list = new List<ChildAuthorize>();
            var rows = result.Split('\n');
            foreach (var row in rows)
            {
                if (string.IsNullOrEmpty(row)) continue;

                var fields = row.Split(new[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                list.Add(new ChildAuthorize
                {
                    StockCode = fields[1],
                    StockName = fields[2],
                    TradeFlag = fields[4] == "买入" ? "1" : "0",
                    AuthorizeState = fields[5],
                    AuthorizePrice = float.Parse(fields[6]),
                    AuthorizeCount = int.Parse(fields[7]),
                    AuthorizeIndex = fields[8],
                    StrikeCount = int.Parse(fields[9]),
                    StrikePrice = float.Parse(fields[11]),
                });
            }
            return list;
        }

        /// <summary>
        /// 获取本地所有委托
        /// </summary>
        public List<ChildAuthorize> GetList(int childId, DateTime beginTime)
        {
            using (var db = GetDbContext())
            {
                var beginTimeValue = beginTime.ToUnixTime();
                return db.ChildAuthorizes.Where(e => e.ClientID == childId && e.AuthorizeTimeValue > beginTimeValue).ToList();
            }
        }
    }
}
