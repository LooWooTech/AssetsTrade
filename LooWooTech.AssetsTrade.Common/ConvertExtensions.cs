using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Common
{
    public static class ConvertExtensions
    {
        public static DateTime ToDate(this string str)
        {
            var year = 0;
            var month = 0;
            var day = 0;
            int.TryParse(str.Substring(0, 4), out year);
            int.TryParse(str.Substring(4, 2), out month);
            int.TryParse(str.Substring(6), out day);

            return new DateTime(year, month, day);
        }
    }
}
