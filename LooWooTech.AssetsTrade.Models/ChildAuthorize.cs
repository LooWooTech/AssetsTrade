using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LooWooTech.AssetsTrade.Common;


namespace LooWooTech.AssetsTrade.Models
{
    /// <summary>
    /// 子帐户委托表
    /// </summary>
    [Table("ChildAuthorize")]
    public class ChildAuthorize
    {
        [Key]
        [MaxLength(20)]
        public string ID { get; set; }
        /// <summary>
        /// 子帐号ID，外键，与子帐号表中的主键关联
        /// </summary>
        public int ClientID { get; set; }
        /// <summary>
        /// 委托时间
        /// </summary>
        [Column("AuthorizeTime")]
        public int AuthorizeTimeValue
        {
            get
            {
                return _authorizeTime.ToUnixTime();
            }
            set
            {
                _authorizeTime = value.ToDateTime();
            }
        }

        private DateTime _authorizeTime;
        [NotMapped]
        public DateTime AuthorizeTime
        {
            get
            {
                return _authorizeTime;
            }
            set
            {
                _authorizeTime = value;
            }
        }
        /// <summary>
        /// 证券代码
        /// </summary>
        public string StockCode { get; set; }
        /// <summary>
        /// 证券名称
        /// </summary>
        public string StockName { get; set; }
        /// <summary>
        /// 委托数量
        /// </summary>
        public int AuthorizeCount { get; set; }
        /// <summary>
        /// 委托价格
        /// </summary>
        public float AuthorizePrice { get; set; }
        /// <summary>
        /// 未报、待报、正报、已报、废单、部成、已成、部撤、已撤(被撤委托)、
        /// 待撤(被撤委托)、未撤(撤单委托)、待撤(撤单委托)、正撤(撤单委托)、
        /// 撤认(撤单委托)、撤废(撤单委托)、已撤(撤单委托)
        /// </summary>
        public string AuthorizeState { get; set; }
        /// <summary>
        /// 买卖标志，0表示卖出，1表示买入
        /// </summary>
        public string TradeFlag { get; set; }
        /// <summary>
        /// 委单号
        /// </summary>
        public string AuthorizeIndex { get; set; }
        /// <summary>
        /// 成交价格
        /// </summary>
        public float StrikePrice { get; set; }
        /// <summary>
        /// 成交数量
        /// </summary>
        public int StrikeCount { get; set; }
        /// <summary>
        /// 撤单数量
        /// </summary>
        public int UndoCount { get; set; }
        /// <summary>
        /// 成交时间
        /// </summary>
        public int TradeTime { get; set; }
        /// <summary>
        /// 子帐户佣金比例
        /// </summary>
        public float ChildCommission { get; set; }
        /// <summary>
        /// 主帐户佣金比例
        /// </summary>
        public float MainCommission { get; set; }
        /// <summary>
        /// 主帐户印花税比例
        /// </summary>
        public float MainYinHuaShui { get; set; }
        /// <summary>
        /// 主帐户过户费
        /// </summary>
        public float MainGuoHuFei { get; set; }
        /// <summary>
        /// 资金余额
        /// </summary>
        public float OverFlowMoney { get; set; }
    }
}
