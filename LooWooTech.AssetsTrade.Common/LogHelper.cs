using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Common
{
    public class LogHelper
    {
        public static void WriteLog(Exception ex)
        {
            try
            {
                var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
                if (!Directory.Exists(logPath))
                {
                    Directory.CreateDirectory(logPath);
                }
                var content = new StringBuilder();
                content.AppendLine(ex.Message);
                content.AppendLine(ex.StackTrace);
                content.AppendLine(ex.Source);
                File.WriteAllText(Path.Combine(logPath, ex.GetType().Name + DateTime.Now.Ticks.ToString() + ".txt"), content.ToString());
            }
            catch
            {

            }
        }

    }
}
