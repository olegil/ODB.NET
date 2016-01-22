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
  
            Publish pub = new Publish() { Name = "Bloger", Address = new Address() { Flat = "64", Street = "ABC", City = "Hong Kong" } };

            Book book = new Book()
            {
                ISBN = "JTSEWAGEASg-3457242",
                Release = DateTime.Now,
                Publish = pub,
                User = new User() { Name = "Peter", BID = "439D5284-7A10-464B-9935-B8B7A49D309A" }
            };      

            int a = db.Store(book);

            db.Close();

            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestStore2()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            db.Depth = 3;

            User user = db.Select<User>().Where("Name").Eq("Chan").First();

            int a = 0;

            if (user != null)
            {
                user.BID = "5B4C7076-97F5-46F0-BE0B-18ACFE654472";

                a = db.Store(user);
            }

            db.Close();

            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestStore3()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            db.Depth = 3;

            db.Clear<User>();

            Book book = db.Select<Book>().First();

            int a = 0;

            if (book != null)
            {
                book.Release = DateTime.Now;

                a = db.Store(book);
            }

            db.Close();

            Assert.IsTrue(a > 0);
        }
    }
}
