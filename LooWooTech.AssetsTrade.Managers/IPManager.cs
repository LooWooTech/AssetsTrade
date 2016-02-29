using LooWooTech.AssetsTrade.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers
{
    public class IPManager : ManagerBase
    {
        private static DateTime _lastUpdateTime = DateTime.MinValue;
        private static readonly List<ServiceIP> IPList = new List<ServiceIP>();
        private static object _lockObj = new object();

        public List<ServiceIP> GetList()
        {
            using (var db = GetDbContext())
            {
                return db.ServiceIPs.ToList();
            }
        }

        public void UpdateIPList()
        {
            if ((DateTime.Now - _lastUpdateTime).TotalHours < 1) return;

            lock (_lockObj)
            {
                if ((DateTime.Now - _lastUpdateTime).TotalHours < 1) return;

                var list = GetList();

                if (list.Count == 0)
                {
                    throw new Exception("请在数据库中配置Service的IP和端口");
                }

                IPList.Clear();

                if (list.Count == 1)
                {
                    IPList.Add(list[0]);
                    return;
                }

                new Thread(() =>
                {
                    foreach (var ip in list)
                    {
                        var ping = new System.Net.NetworkInformation.Ping();
                        var reply = ping.Send(ip.IPAddress, 1000 * 10);
                        if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                        {
                            ip.Ping = reply.RoundtripTime;
                            IPList.Add(ip);
                        }
                    }
                }).Start();
                _lastUpdateTime = DateTime.Now;
            }
        }

        public ServiceIP GetFastIP(Type serviceType = null)
        {
            UpdateIPList();
            var serviceName = serviceType.Name;
            return IPList.Where(e => e.Service == serviceName).OrderBy(e => e.Ping).FirstOrDefault();
        }

    }
}
