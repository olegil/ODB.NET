using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.ODB;
using System.Data.ODB.Linq;
using System.Data.ODB.SQLite;
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
                     select new { PName = u.Name, PAge = u.Age };

            var q2 = q1.Skip(1);

            string sql = q1.ToString();

            var list = q1.ToList();

            respo.Dispose();

            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestSkip()
        {
            //SqlContext db = new SqlContext(Command.MssqlConnStr);
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Depth = 2;

            IQuery<Order> q1 = db.Query<Order>().Skip(4);

            IQuery<User> q2 = db.Query<User>().Take(2);

            IQuery<User> q3 = db.Query<User>().Skip(1).Take(2);

            IQuery<User> q4 = db.Query<User>().Take(2).Skip(1);

            List<User> list = q2.ToList();

            Assert.IsTrue(list.Count > 0);
        }
    }
}
