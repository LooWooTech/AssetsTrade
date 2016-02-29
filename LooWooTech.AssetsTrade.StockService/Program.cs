using LooWooTech.AssetsTrade.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.StockService
{
    static class Program
    {
        private static AuthorizeService service1 = new AuthorizeService();
        private static CloseAccountService service2 = new CloseAccountService();
        private static RefreshDataService service3 = new RefreshDataService();

        private static void StartService()
        {
            service1.Start();
            service2.Start();
            service3.Start();
        }

        private static void StopService()
        {
            service1.Stop();
            service2.Stop();
            service3.Stop();
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            Console.ForegroundColor = ConsoleColor.White;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

#if DEBUG
            StartService();
            LogWriter.Success("服务已启动");
            while (true)
            {
                var cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "stop":
                        StopService();
                        break;
                    case "start":
                        StartService();
                        break;
                    case "help":
                        Console.WriteLine("start|stop|exit");
                        break;
                    case "exit":
                        break;
                }
            }

#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            LogWriter.Error(ex.Message);
            LogHelper.WriteLog(ex);
        }
    }
}
