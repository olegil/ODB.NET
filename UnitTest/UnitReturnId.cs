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

            Publish pub = new Publish() { Name = "Bloger", Address = new Address() { Street = "Queen Road", Flat = "Westland" } };

            long i = db.Insert(pub);

            Assert.IsTrue(i != 0);
        }
    }
}
