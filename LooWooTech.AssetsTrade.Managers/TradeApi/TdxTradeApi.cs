using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers.TradeApi
{
    internal static class TdxTradeApi
    {
        [DllImport("TdxTrade.dll")]
        public static extern void SetServer(string szHost, int nPort);

        /// <summary>
        /// 设置账号信息
        /// </summary>
        /// <param name="szAccount">用户账号</param>
        /// <param name="szPassword">交易密码</param>
        /// <param name="szVerifyCode">通讯密码</param>
        [DllImport("TdxTrade.dll")]
        public static extern void SetAccount(string szAccount, string szPassword, string szVerifyCode);
        /// <summary>
        /// 登录（登录成功才可以交易）
        /// </summary>
        /// <returns>true|false 登录成功或失败</returns>
        [DllImport("TdxTrade.dll")]
        public static extern bool Login();

        /// <summary>
        /// 每次调用接口后如果失败了 可以调用这个函数获取服务器的一些信息
        /// </summary>
        /// <returns></returns>
        [DllImport("TdxTrade.dll")]
        public static extern string GetReturnInfo();
        /// <summary>
        /// 下买单
        /// </summary>
        /// <param name="sCode">股票代码</param>
        /// <param name="sNum">购买数量</param>
        /// <param name="sPrice">买入价格（2位小数）</param>
        /// <param name="Result"></param>
        /// <param name="ErrInfo"></param>
        /// <returns></returns>
        [DllImport("TdxTrade.dll")]
        public static extern int ToBuy(string sCode, int sNum, double sPrice, StringBuilder Result, StringBuilder ErrInfo);
        /// <summary>
        /// 下卖单
        /// </summary>
        /// <param name="sCode">股票代码</param>
        /// <param name="sNum">购买数量</param>
        /// <param name="sPrice">买入价格（2位小数）</param>
        /// <param name="Result"></param>
        /// <param name="ErrInfo"></param>
        /// <returns></returns>
        [DllImport("TdxTrade.dll")]
        public static extern int ToSell(string sCode, int sNum, double sPrice, StringBuilder Result, StringBuilder ErrInfo);
        /// <summary>
        /// 撤单指令
        /// </summary>
        /// <param name="sCode">股票代码</param>
        /// <param name="szNumber">下单后得到的委托号</param>
        /// <param name="Result"></param>
        /// <param name="ErrInfo"></param>
        [DllImport("TdxTrade.dll")]
        public static extern void CancelOrder(string sCode, string szNumber, StringBuilder Result, StringBuilder ErrInfo);
        /// <summary>
        /// 查询委托
        /// </summary>
        /// <param name="flag">查询持仓 0  委托 1   成交 2   资金  3</param>
        /// <param name="Result">
        /// 持仓：0股票代码 1股票名称 2股票数量 3可卖数量 4成本价 5当前价 6最近市值 7浮动盈亏 8盈亏比例 9股东代码
        /// 委托：0时间 1股票代码 2股票名称 3、0 4买卖标志 5状态说明 6委托价格 7委托数量 8委托编号 9成交数量 10成交均价 11成交金额 12报价方式 13股东代码
        /// 成交：
        /// </param>
        /// <param name="ErrInfo"></param>
        [DllImport("TdxTrade.dll")]
        public static extern void QueryData(int flag, StringBuilder Result, StringBuilder ErrInfo);
        /// <summary>
        /// 查询历史成交
        /// </summary>
        /// <param name="start">查询的开始日期</param>
        /// <param name="end">查询结束日期</param>
        /// <param name="Result"></param>
        /// <param name="ErrInfo"></param>
        [DllImport("TdxTrade.dll")]
        public static extern void QueryHistoryData(string start, string end, StringBuilder Result, StringBuilder ErrInfo);

        /// <summary>
        /// 查询资金流水
        /// </summary>
        /// <param name="start">查询的开始日期</param>
        /// <param name="end">查询结束日期</param>
        /// <param name="Result"></param>
        /// <param name="ErrInfo"></param>
        [DllImport("TdxTrade.dll")]
        public static extern void QueryHistoryMoney(string start, string end, StringBuilder Result, StringBuilder ErrInfo);
    }
}
