using Common.Caches;
using Common.Helper;
using Common.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTestProject1
{
    [TestClass]
    public class RedisTest
    {
        public static IConfiguration Configuration { get; set; }
        [TestMethod]
        public void TestMethod()
        {  
            ICache cache = new Redis();

            var pathes = cache.Get<string>("test");

        }
    }
}
