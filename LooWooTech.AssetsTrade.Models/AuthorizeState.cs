using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Models
{
    public enum BOOL
    {
        TRUE = 0,
        FALSE = 1
    }

    public enum AuthorizeState
    {
        未报, 待报, 正报, 已报, 废单, 部成, 已成, 部撤, 被撤已撤, 被撤待撤, 未撤, 待撤, 正撤, 撤认, 撤废, 已撤
    }
}
