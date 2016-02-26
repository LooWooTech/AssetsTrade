using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.StockService
{
    internal class RefreshDataService : ServiceBase
    {
        private bool _hasRefreshed;

        protected override void Dowork()
        {
            if (_hasRefreshed)
            {
                if (DateTime.Now.Minute != ServiceWorkTime.RefreshTime.Minute)
                {
                    _hasRefreshed = false;
                }
                return;
            }
            if (DateTime.Now.Hour == ServiceWorkTime.RefreshTime.Hour && DateTime.Now.Minute == ServiceWorkTime.RefreshTime.Minute)
            {
                //怎么刷新数据？

                _hasRefreshed = true;
            }
        }

        protected override int GetInterval()
        {
            return ServiceWorkTime.IsWorkingTime ? 1200 : 30;
        }
    }
}
