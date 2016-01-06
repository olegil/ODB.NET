using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitInsert
    {
        [TestMethod]
        public void TestAdd()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            db.Clear<User>();
            db.Clear<Address>();

            User user = new User() { Name = "Peter", Birthday = DateTime.Now, Address = new Address() { Flat = "64", Street = "Queen Road", City = "HK" } };
                    
            int a = db.Insert(user);
 
            db.Close();

            Assert.IsTrue(a > 0);
        }
    }
}