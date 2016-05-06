using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitReturnId
    {
        [TestMethod]
        public void TestInsertId()
        {
            MyRepository db = new MyRepository();

            Publish pub = new Publish() { Name = "Bloger", Address = new Address() { Street = "Queen Road", Flat = "Westland" } };

            db.Store(pub);

            Assert.IsTrue(pub.Id > 0);
        }
    }
}
