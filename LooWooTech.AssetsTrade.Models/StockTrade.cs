using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Models
{
    /// <summary>
    /// 股票成交
    /// </summary>
    public class StockTrade
    {
        public static StockTrade Parse(string queryData)
        {
            //当日成交
            //0         1          2    3     4       5       6          7成交编号  8委托编号       9           10
            //002015    霞客环保    0   买入   9.560   600.00  5736.00    19698    19370          0107874749  0
            //000856    冀东装备    1   卖出   10.730  100.00  1073.00    19506    19206          0107874749  0
            //历史成交
            //0           1       2          3   4       5        6            7       8       9          10   11
            //20160225    600839  四川长虹    1   卖出    4.020   - 1000.00     18177   2209    A474859797  1   4020.00
            //20160225    600839  四川长虹    1   卖出    4.020   - 8000.00     18178   2244    A474859797  1   32160.00
            //20160225    600528  中铁二局    0   买入    14.900    3500.00     18180   3227    A474859797  1   52150.00
            if (string.IsNullOrEmpty(queryData)) return null;
            var fields = queryData.Split('\t');
            //当日
            if (fields.Length == 10)
            {
                return new StockTrade
                {
                    StockCode = fields[0],
                    StockName = fields[1],
                    TradeFlag = fields[3],
                    StrikePrice = double.Parse(fields[4]),
                    StrikeCount = int.Parse(fields[5]),

                    TradeID = fields[7],
                    AuthorizeIndex = fields[8],
                    StockHolderCode = fields[9],
                    TradeDate = DateTime.Today,
                };
            }
            else
            {
                var year = int.Parse(fields[0].Substring(0, 4));
                var month = int.Parse(fields[0].Substring(4, 2));
                var day = int.Parse(fields[0].Substring(6));
                return new StockTrade
                {
                    StockCode = fields[1],
                    StockName = fields[2],
                    TradeFlag = fields[4],
                    StrikePrice = double.Parse(fields[5]),
                    StrikeCount = int.Parse(fields[6]),

                    TradeID = fields[7],
                    AuthorizeIndex = fields[8],
                    StockHolderCode = fields[9],
                    TradeDate = new DateTime(year, month, day)
                };
            }
        }

        public string StockCode { get; set; }

        public string StockName { get; set; }

        /// <summary>
        /// 成交编号
        /// </summary>
        public string TradeID { get; set; }

        /// <summary>
        /// 委托编号
        /// </summary>
        public string AuthorizeIndex { get; set; }
        /// <summary>
        /// 股东代码
        /// </summary>
        public string StockHolderCode { get; set; }

        /// <summary>
        /// 成交金额
        /// </summary>
        public double StrikeMoney                                                                                    
        {
            get
            {
                return Math.Abs(StrikeCount * StrikePrice);
            }
        }
        /// <summary>
        /// 成交价格
        /// </summary>
        public double StrikePrice { get; set; }

        /// <summary>
        /// 成交数量
        /// </summary>
        public int StrikeCount { get; set; }
        /// <summary>
        /// 成交日期
        /// </summary>
        public DateTime TradeDate { get; set; }

        /// <summary>
        /// 操作类型（买入、卖出）
        /// </summary>
        public string TradeFlag { get; set; }

    }
}
