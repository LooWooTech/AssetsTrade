using LooWooTech.AssetsTrade.Managers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.StockService
{
    internal class AuthorizeService : ServiceBase
    {

        protected override int GetInterval()
        {
            return ServiceWorkTime.IsWorkingTime ? 1 : 120;
        }

        protected override void Dowork()
        {
            var list = Core.ChildAuthorizeManager.GetTodayAuthorize();
            //更新委托
            foreach (var item in list)
            {
                Core.TradeManager.UpdateAuthorize(item);
            }
        }
    }
}
