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
 
            User user = new User() { Name = "Stephen", Birthday = DateTime.Now };

            int a = db.Store(user);

            db.Close();

            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestStore2()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            //Address addr = db.From<Address>().First();

            User user = db.From<User>().Where("Name").Eq("Stephen").First();

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
