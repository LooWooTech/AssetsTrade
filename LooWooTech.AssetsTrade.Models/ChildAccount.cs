﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Models
{
    /// <summary>
    /// 子帐户表
    /// </summary>
    [Table("ChildAccountCode")]
    public class ChildAccount
    {
        /// <summary>
        /// 子帐号，主键
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChildID { get; set; }
        /// <summary>
        /// 子帐号名称
        /// </summary>
        [MaxLength(20)]
        public string ChildName { get; set; }
        /// <summary>
        /// 父ID，外键，与主帐号表中的MainID关联
        /// </summary>
        public string ParentID { get; set; }

        [NotMapped]
        public MainAccount Parent { get; set; }
        /// <summary>
        /// 子帐户密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 保证金(可变)
        /// </summary>
        public float HandMoney { get; set; }
        /// <summary>
        /// 融资金额
        /// </summary>
        public float BorrowMoney { get; set; }
        /// <summary>
        /// 可用余额
        /// </summary>
        public float UseableMoney { get; set; }
        /// <summary>
        /// 期初分配金额(划拨进来的总资产-划拨出去的总资产)
        /// </summary>
        public float FirsthandMoney { get; set; }
        /// <summary>
        /// 佣金，双边收
        /// </summary>
        public float Commission { get; set; }
        /// <summary>
        /// 印花税，出让方单边收
        /// </summary>
        public float YinHuaShui { get; set; }
        /// <summary>
        /// 过户费，上海双边收
        /// </summary>
        public float GuoHuFei { get; set; }
        /// <summary>
        /// 是否最低收一元，不确定不要修改，0表示不是，1表示是
        /// </summary>
        public int IsOneMoney { get; set; }
        /// <summary>
        /// 是否允许买卖，0不允许，1允许
        /// </summary>
        public int IsAllowableTrade { get; set; }
        /// <summary>
        /// 资产交易比例
        /// </summary>
        public float AssetsTradeScale { get; set; }
        /// <summary>
        /// 单支股票持仓比例
        /// </summary>
        public float HaveScale { get; set; }
        /// <summary>
        /// 创业板股票持仓比例
        /// </summary>
        public float StartBusinessScale { get; set; }
        public int IsLogin { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(200)]
        public string Memo { get; set; }
        /// <summary>
        /// 强平线
        /// </summary>
        public float QiangPingLine { get; set; }
        /// <summary>
        /// 预警线
        /// </summary>
        public float YuJingLine { get; set; }
        /// <summary>
        /// 是否强平，0不强平，1强平
        /// </summary>
        public int ISQiangPing { get; set; }
        /// <summary>
        /// 是否最低五元佣金，0表示不是，1表示是
        /// </summary>
        public int ISLowFiveMoney { get; set; }
        public int IsNew { get; set; }
        public int IsST { get; set; }
        public int IsChuangYeBan { get; set; }

        public float GetShouXuFei(string stockCode,float price, int number)
        {
            var result = (price * number) * Commission;
            if (ISLowFiveMoney == 1 && result < 5)
            {
                result = 5;
            }
            return result;
        }

        public float GetYinHuaShui(string stockCode,float price,int number)
        {
            if(stockCode.StartsWith("1"))
            {
                return 0;   
            }
            return price * number * YinHuaShui;
        }

        public float GetGuoHuFei(string stockCode,float price, int number)
        {
            if (stockCode.StartsWith("6"))
            {
                return  (price * number) * GuoHuFei;
            }
            return 0;
        }
    }
}
