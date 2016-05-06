using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Data.ODB.MSSQL;
using System.Data.SQLite;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitTable
    {
        [TestMethod]
        public void TestTable()
        {
            MyRepository respo = new MyRepository();
             
            respo.Remove<Order>();
            respo.Create<Order>();

            Order order = new Order() { PackageID = "3452347-57452357", User = new User() { Name = "Peter", BID = 2445.235d, IsPermit = false, Birthday = DateTime.Now }, Date = DateTime.Now };

            respo.Store(order);
 
            Assert.IsTrue(order.Id > 0);
        }

        [TestMethod]
        public void TestClear()
        {
            MyRepository respo = new MyRepository();

            respo.Clear<Order>();

            int a = 0;

            Assert.IsTrue(a > 0);
        }
    }
}
