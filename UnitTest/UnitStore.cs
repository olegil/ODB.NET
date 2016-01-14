using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitStore
    {
        [TestMethod]
        public void TestStore()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));
 
            User user = new User() { Name = "Peter", Birthday = DateTime.Now, Address = new Address() { City = "HK", Flat = "12", Street = "Queen" } };
            
            Book book = new Book() { ISBN = "JTSEWAGEASg-3457242", Release = DateTime.Now };

            int a = db.Store(book);

            db.Close();

            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestStore2()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            db.Depth = 3;

            User user = db.Get<User>().Where("Name").Eq("Chan").First();

            int a = 0;

            if (user != null)
            {
                user.Address = new Address() { Street = "Westland Road" };

                a = db.Store(user);
            }

            db.Close();

            Assert.IsTrue(a > 0);
        }
    }
}
