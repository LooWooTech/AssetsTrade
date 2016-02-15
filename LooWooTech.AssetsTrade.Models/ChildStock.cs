using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Models
{
    /// <summary>
    /// 子帐户持仓表
    /// </summary>
    [Table("ChildStock")]
    public class ChildStock
    {
        [Key]
        public string ID { get; set; }
        /// <summary>
        /// 子帐号ID，外键，与子帐号表中的主键关联
        /// </summary>
        public int ChildID { get; set; }
        /// <summary>
        /// 证券代码
        /// </summary>
        [MaxLength(6)]
        public string StockCode { get; set; }
        /// <summary>
        /// 证券名称
        /// </summary>
        [MaxLength(10)]
        public string StockName { get; set; }
        /// <summary>
        /// 期初数量，当前业务日期的初始数量
        /// </summary>
        public int InitCount { get; set; }
        /// <summary>
        /// 当前该支证券的所有数量，买卖时都要修改该字段
        /// </summary>
        public int AllCount { get; set; }
        /// <summary>
        /// 从证券公司查询回来的可用数量，买卖时都要修改该字段
        /// </summary>
        public int UseableCount { get; set; }
        /// <summary>
        /// 上次同步时间
        /// </summary>
        public int LastTime { get; set; }
        /// <summary>
        /// 成本价
        /// </summary>
        public float PrimeCost { get; set; }
        /// <summary>
        /// 市价
        /// </summary>
        public float CurrentPrice { get; set; }
    }
}
