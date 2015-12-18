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

            db.Create<User>();

            User user = new User() { Name = "Stephen" };                   

            db.Insert(user);

           int n = db.Table<User>().ToList().Count;
             
            db.Close();

            Assert.IsTrue(n > 0);
        }
    }
}
