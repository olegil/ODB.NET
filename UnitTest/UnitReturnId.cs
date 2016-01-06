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
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            User user = new User() { Name = "Stephen", Birthday = DateTime.Parse("1991/8/16"), Address = new Address() { City = "HK", Street = "Queen Road", Flat = "Westland" } };

            long i = db.Insert(user);

            Assert.IsTrue(i != 0);
        }
    }
}
