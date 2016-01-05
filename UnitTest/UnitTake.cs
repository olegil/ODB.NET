using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.ODB;
using System.Data.ODB.Linq;

namespace UnitTest
{
    [TestClass]
    public class UnitTake
    {
        [TestMethod]
        public void TestTake()
        {
            MyRepository respo = new MyRepository();

            var q1 = from u in respo.Users
                     select u;

            var q2 = q1.Take(2).Skip(1);

            string sql = q2.ToString();

            List<User> list = q2.ToList();

            respo.Dispose();

            Assert.IsTrue(list.Count > 0);
        }
    }
}
