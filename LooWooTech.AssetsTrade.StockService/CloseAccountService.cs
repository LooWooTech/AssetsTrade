﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.StockService
{
    internal class CloseAccountService : ServiceBase
    {
        private bool _hasCloseAccount;

        protected override void Dowork()
        {
            if(_hasCloseAccount)
            {
                if(DateTime.Now.Minute != ServiceWorkTime.CloseAccountTime.Minute)
                {
                    _hasCloseAccount = false;
                }
                return;
            }
            if (DateTime.Now.Hour == ServiceWorkTime.CloseAccountTime.Hour && DateTime.Now.Minute == ServiceWorkTime.CloseAccountTime.Minute)
            {
                //结算所有委托
                Core.ChildAuthorizeManager.CloseAllAuthorize();
                _hasCloseAccount = true;
            }
        }

        protected override int GetInterval()
        {
            return 30;
        }
    }
}
