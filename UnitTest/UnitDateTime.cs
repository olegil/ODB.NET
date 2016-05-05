using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.ODB;
using System.Data.ODB.Linq;
using System.Data.ODB.SQLite;
using System.Data.SQLite;

namespace UnitTest
{
    [TestClass]
    public class UnitDateTime
    {
        [TestMethod]
        public void TestDateTime()
        { 
            MyRepository respo = new MyRepository();

            DateTime dt = DateTime.Parse("2016-01-10");

            var query = from u in respo.Users
                        where u.Birthday > dt
                        select u;

            List<User> list = query.ToList();
 
            Assert.IsTrue(list.Count > 0);
        }
    }
}
