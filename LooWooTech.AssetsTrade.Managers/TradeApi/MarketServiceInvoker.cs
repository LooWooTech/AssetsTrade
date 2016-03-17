﻿using LooWooTech.AssetsTrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers.TradeApi
{
    public class MarketServiceInvoker
    {
        private Dictionary<IMarketService, bool> _services = new Dictionary<IMarketService, bool>();

        public MarketServiceInvoker()
        {
            _services.Add(new TdxMarketService(), true);
        }

        private IMarketService GetTradeService()
        {
            return _services.FirstOrDefault(e => e.Value).Key;
        }

        private void SetMainService(IMarketService service)
        {
            foreach (var kv in _services)
            {
                _services[kv.Key] = kv.Key.GetType().Name == service.GetType().Name;
            }
        }

        private static ApiResult InvokeApi(IMarketService service, string method, object[] arguments)
        {
            var host = ManagerCore.Instance.ApiHostManager.GetFastHost(service.GetType(), ApiType.Market);
            service.Connect(host);

            var result = (ApiResult)service.GetType().InvokeMember(method, System.Reflection.BindingFlags.InvokeMethod, null, service, arguments);
            if (!string.IsNullOrEmpty(result.Error))
            {
                //尝试一次登录
                if (result.Error.Contains("连接已断开"))
                {
                    service.Disconnet();
                    service.Connect(host);
                    result = (ApiResult)service.GetType().InvokeMember(method, System.Reflection.BindingFlags.InvokeMethod, null, service, arguments);
                }
            }
            return result;
        }

        public ApiResult InvokeMethod(string method, object[] arguments)
        {
            var service = GetTradeService();
            var result = InvokeApi(service, method, arguments);
            //如果出错，调用其它服务
            if (!string.IsNullOrEmpty(result.Error))
            {
                foreach (var kv in _services)
                {
                    if (kv.Key.GetType() == service.GetType())
                    {
                        continue;
                    }
                    if (!kv.Value)
                    {
                        result = InvokeApi(kv.Key, method, arguments);
                        if (string.IsNullOrEmpty(result.Error))
                        {
                            SetMainService(kv.Key);
                            break;
                        }
                    }
                }
            }
            return result;
        }
    }
}
