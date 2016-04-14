using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.ODB;
using System.Data.ODB.Linq;
using System.Data.ODB.MSSQL;

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
                     where u.Age == 23
                     select u;

            var q2 = q1.Take(2).Skip(1);

            string sql = q2.ToString();

            List<User> list = q2.ToList();

            respo.Dispose();

            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestSkip()
        {
            SqlContext db = new SqlContext(Command.MssqlConnStr);

            IQuery<Order> q = db.Query<Order>().Skip(1).Take(2);

            List<Order> list = q.ToList();

            Assert.IsTrue(list.Count > 0);
        }
    }
}
