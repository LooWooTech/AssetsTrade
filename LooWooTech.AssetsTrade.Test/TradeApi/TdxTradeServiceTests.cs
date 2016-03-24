using Microsoft.VisualStudio.TestTools.UnitTesting;
using LooWooTech.AssetsTrade.Managers.TradeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LooWooTech.AssetsTrade.Models;
using System.Diagnostics;
using LooWooTech.AssetsTrade.TradeApi;

namespace LooWooTech.AssetsTrade.Managers.TradeApi.Tests
{
    [TestClass()]
    public class TdxTradeServiceTests
    {
        private readonly TdxTradeService Service = new TdxTradeService();

        private MainAccount _mainAccount;
        private ApiHost _ip;

        public TdxTradeServiceTests()
        {
            _mainAccount = new MainAccount
            {

            };
            _ip = ManagerCore.Instance.ApiHostManager.GetFastHost(Service.GetType());
        }

        private void Login()
        {
            Service.Login();
        }

        [TestMethod()]
        public void LoginTest()
        {
            var result = Service.Login();
            Assert.AreEqual(true, result.Result);
        }

        [TestMethod()]
        public void QueryAuthroizesTest()
        {
            Login();

            var result = Service.QueryAuthroizes();
            Console.WriteLine(result.Result);
            Assert.AreEqual(true, result.Result);
        }

        [TestMethod()]
        public void QueryStocksTest()
        {
            Login();

            var result = Service.QueryStocks();
            Trace.WriteLine(result.Data);
            Assert.AreEqual(true, result.Result);
        }

        [TestMethod()]
        public void QueryMoneyTest()
        {
            Login();

            var result = Service.QueryMoney();
            Trace.WriteLine(result.Data);
            Assert.AreEqual(true, result.Result);
        }

        [TestMethod()]
        public void QueryTradesTest()
        {
            Login();

            var result = Service.QueryTrades();
            Trace.WriteLine(result.Data);
            Assert.AreEqual(true, result.Result);
        }

        [TestMethod()]
        public void QueryHistoryTradeTest()
        {
            Login();

            var result = Service.QueryHistoryTrade(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
            Trace.WriteLine(result.Data);
            Assert.AreEqual(true, result.Result);
        }

        [TestMethod()]
        public void QueryHistoryMoneyTest()
        {
            Login();

            var result = Service.QueryHistoryMoney(DateTime.Today.AddDays(-1), DateTime.Today.AddDays(1));
            Trace.WriteLine(result.Data);
            Assert.AreEqual(true, result.Result);
        }

        [TestMethod]
        public void QueryQuotesTest()
        {
            //Service.QueryQuotes();
        }
    }
}