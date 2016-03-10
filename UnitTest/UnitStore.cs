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

            OrderItem item = db.Get<OrderItem>().First();

            int a = 0;

            if (item != null)
            {
                item.Name = "Pencil";
                item.ItemId = 10023;

                a = db.Store(item);
            }

            db.Close();

            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestStore3()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            OrderItem item = new OrderItem() { Name = "ABC", OrderId = 1, ItemId = 10235235, Price = 55.99m };

            int a = db.Insert(item);      

            db.Close();

            Assert.IsTrue(a > 0);
        }
    }
}
