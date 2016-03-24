﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using LooWooTech.AssetsTrade.Managers.TradeApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LooWooTech.AssetsTrade.TradeApi;

namespace LooWooTech.AssetsTrade.Managers.TradeApi.Tests
{
    [TestClass()]
    public class TdxMarketServiceTests
    {

        private TdxMarketService TdxMarketService = new TdxMarketService();

        [TestMethod()]
        public void GetMarketInfoTest()
        {
            var host = ManagerCore.Instance.ApiHostManager.GetFastHost(typeof(TdxMarketService));

            TdxMarketService.Connect(host);

            var result = TdxMarketService.GetMarketInfo(new[] { "603077" });

            TdxMarketService.Disconnet();

            Assert.AreEqual(true, result.Result);
        }
    }
}