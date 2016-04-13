using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitCount
    {
        [TestMethod]
        public void TestCount()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            User user = new User() { BID = 2345F, Name = "Chan Peter" };

            db.Insert(user);

            int n = db.Count<User>(); 
 
            db.Close();

            Assert.IsTrue(n > 0);

        }
    }
}
