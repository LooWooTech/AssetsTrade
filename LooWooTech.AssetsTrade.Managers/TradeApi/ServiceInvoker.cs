using LooWooTech.AssetsTrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers.TradeApi
{
    public class ServiceInvoker
    {
        private Dictionary<ITradeService, bool> _services = new Dictionary<ITradeService, bool>();

        public ServiceInvoker()
        {
            _services.Add(new TdxTradeService(), true);
        }

        private ITradeService GetService()
        {
            return _services.FirstOrDefault(e => e.Value).Key;
        }

        private void SetMainService(ITradeService service)
        {
            foreach (var kv in _services)
            {
                _services[kv.Key] = kv.Key.GetType().Name == service.GetType().Name;
            }
        }

        private ApiResult InvokeApi(ITradeService service, string method, object[] arguments)
        {
            var serviceIp = ManagerCore.Instance.ServiceIPManager.GetFastIP(service.GetType());
            var mainAccount = ManagerCore.Instance.MainAccountManager.GetServerMainAccount();
            service.Login(mainAccount, serviceIp);

            var result = (ApiResult)service.GetType().InvokeMember(method, System.Reflection.BindingFlags.InvokeMethod, null, service, arguments);
            if (!string.IsNullOrEmpty(result.Error))
            {
                //尝试一次登录
                if (result.Error.Contains("网络连接失败"))
                {
                    service.Logout();
                    service.Login(mainAccount, serviceIp);
                    result = (ApiResult)service.GetType().InvokeMember(method, System.Reflection.BindingFlags.InvokeMethod, null, service, arguments);
                }
            }
            return result;
        }

        public ApiResult InvokeMethod(string method, object[] arguments)
        {
            var service = GetService();
            var result = InvokeApi(service, method, arguments);
            //如果出错，调用其它服务
            if (!string.IsNullOrEmpty(result.Error))
            {
                foreach (var kv in _services)
                {
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