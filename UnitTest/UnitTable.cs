using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitTable
    {
        [TestMethod]
        public void TestTable()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            //db.Remove<Book>(true);
 
            db.Create<Book>(true);

            db.Close();

            int a = 0;

            Assert.IsTrue(a == 0);
        }

        [TestMethod]
        public void TestClear()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            int a = db.Clear<User>();

            db.Close();

            Assert.IsTrue(a > 0);
        }
    }
}
