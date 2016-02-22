using LooWooTech.AssetsTrade.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.StockService
{
    public class ChildAuthorizeService
    {
        private Thread _worker;
        private static readonly ManagerCore Core = ManagerCore.Instance;
        public void Start()
        {
            _worker = new Thread(() =>
            {
                while (true)
                {
                    Dowork();
                    Thread.Sleep(1000);
                }
            });
            _worker.Start();
        }

        private void Dowork()
        {
            //查询当日委托

            //更新委托

        }

        public void Stop()
        {
            if (_worker != null)
            {
                _worker.Abort();
                _worker = null;
            }
        }
    }
}
