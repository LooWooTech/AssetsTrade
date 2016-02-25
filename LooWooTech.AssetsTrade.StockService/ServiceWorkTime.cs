using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.StockService
{
    public class ServiceWorkTime
    {
        public static DateTime AMStartTime
        {
            get
            {
                return DateTime.Parse(DateTime.Today.ToShortDateString() + "9:30");
            }
        }

        public static DateTime AMStopTime
        {
            get
            {
                return DateTime.Parse(DateTime.Today.ToShortDateString() + "11:30");
            }
        }

        public static DateTime PMStartTime
        {
            get
            {
                return DateTime.Parse(DateTime.Today.ToShortDateString() + "13:00");
            }
        }

        public static DateTime PMStopTime
        {
            get
            {
                return DateTime.Parse(DateTime.Today.ToShortDateString() + "15:00");
            }
        }

        public static DateTime CloseAccountTime
        {
            get
            {
                return DateTime.Parse(DateTime.Today.ToShortDateString() + " " + ConfigurationManager.AppSettings["CloseAccountTime"]);
            }
        }

        public static bool IsWorkingTime
        {
            get
            {
                var now = DateTime.Now;
                return !(now < AMStartTime || (now > AMStopTime && now < PMStartTime) || now > PMStopTime);
            }
        }
    }
}
