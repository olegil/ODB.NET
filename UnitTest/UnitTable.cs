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
            //SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));
            SqlContext db = new SqlContext(Command.MssqlConnStr);

            db.Depth = 2;

            db.Remove<Order>();
            db.Create<Order>();

            Order order = new Order() { PackageID = "3452347-57452357", User = new User() { Name = "Peter", BID = 2445.235d, IsPermit = false, Birthday = DateTime.Now }, Date = DateTime.Now };

            int a = db.Insert(order);

            db.Close(); 

            Assert.IsTrue( a > 0);
        }

        [TestMethod]
        public void TestClear()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Clear<Order>();
                      
            db.Close();

            int a = 0;

            Assert.IsTrue(a > 0);
        }
    }
}
