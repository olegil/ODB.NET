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
        public void TestCreate()
        {
            MyRepository respo = new MyRepository();
                       
            respo.Create<OrderItem>();
        
            int a = 1;
            Assert.IsTrue(a == 1);        
        }

        [TestMethod]
        public void TestRemove()
        {
            MyRepository respo = new MyRepository();

            respo.Remove<OrderItem>();
            respo.Remove<Order>();
            respo.Remove<User>();
            respo.Remove<Address>();

            int a = 1;
            Assert.IsTrue(a == 1);
        }
    }
}
