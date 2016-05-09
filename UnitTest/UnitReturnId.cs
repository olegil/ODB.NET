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

            User user = new User() { Name = "Peter", Balance = 234.564d };

            db.Store(user);

            Assert.IsTrue(user.Id > 0);
        }
    }
}
