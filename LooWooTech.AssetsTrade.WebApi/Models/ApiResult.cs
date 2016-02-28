using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LooWooTech.AssetsTrade.WebApi
{
    public class ApiResult
    {
        public ApiResult() { }

        public int Code { get; set; }

        public object Data { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }
    }
}
