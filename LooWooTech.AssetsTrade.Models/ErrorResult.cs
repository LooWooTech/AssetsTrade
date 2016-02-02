using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LooWooTech.AssetsTrade.Models
{
    [Serializable]
    public class ErrorResult
    {
        public ErrorResult() { }

        public int Code { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }
    }

    [Serializable]
    public class SuccessResult
    {
        public int Code { get; set; }
    }
}
