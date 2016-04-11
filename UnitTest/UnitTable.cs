using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Data.ODB.MSSQL;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitTable
    {
        [TestMethod]
        public void TestTable()
        {
            SqlContext db = new SqlContext(Command.MssqlConnStr);

            db.Depth = 2;

            db.Remove<Order>();
            db.Create<Order>();

            Order oo = new Order() { User = new User() { Name = "Peter", BID = "534262346", Birthday = DateTime.Now }, Date = DateTime.Now };

            int a = db.Insert(oo);

            db.Close(); 

            Assert.IsTrue( a > 0);
        }

        [TestMethod]
        public void TestClear()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Remove<Order>();

            db.Create<Order>();

            db.Close();

            int a = 0;

            Assert.IsTrue(a > 0);
        }
    }
}
