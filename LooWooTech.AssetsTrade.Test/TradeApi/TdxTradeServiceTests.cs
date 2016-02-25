using Microsoft.VisualStudio.TestTools.UnitTesting;
using LooWooTech.AssetsTrade.Managers.TradeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LooWooTech.AssetsTrade.Managers.TradeApi.Tests
{
    [TestClass()]
    public class TdxTradeServiceTests
    {
        private TdxTradeService _service = new TdxTradeService();
        [TestMethod()]
        public void LoginTest()
        {
            var main = ManagerCore.Instance.MainAccountManager.GetServerMainAccount();

            var serverIp = ManagerCore.Instance.ServiceIPManager.GetFastIP(_service.GetType());

            var result = _service.Login(main, serverIp);

            Assert.AreEqual<bool>(true, result.Result);
        }

        [TestMethod()]
        public void QueryAuthroizesTest()
        {
            Assert.Fail();
        }
    }
}