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
        private bool _hasCloseAccount;

        protected override int GetInterval()
        {
            return ServiceWorkTime.IsWorkingTime ? 1 : 60;
        }

        protected override void Dowork()
        {
            if (ServiceWorkTime.IsWorkingTime)
            {
                _hasCloseAccount = false;
                var list = Core.ChildAuthorizeManager.GetTodayAuthorize();
                //更新委托
                foreach (var item in list)
                {
                    Core.TradeManager.UpdateAuthorize(item);
                }
            }
            //如果还没有结算
            else if (!_hasCloseAccount)
            {
                if (DateTime.Now.Hour == ServiceWorkTime.CloseAccountTime.Hour && DateTime.Now.Minute == ServiceWorkTime.CloseAccountTime.Minute)
                {
                    //结算所有委托
                    Core.ChildAuthorizeManager.CloseAllAuthorize();
                }
            }
            else
            {

            }
        }
    }
}
