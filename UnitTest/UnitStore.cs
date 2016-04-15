using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Data.ODB.MSSQL;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitStore
    {
        [TestMethod]
        public void TestStore()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Depth = 2;
 
            Publish pub = new Publish() { Name = "Bloger", Address = new Address() { Flat = "64", Street = "ABC", City = "Hong Kong" } };
            User user = new User() { Name = "Peter", BID = 235f, Birthday = DateTime.Now };

            Book book = new Book()
            {
                ISBN = "JTSEWAGEASg-3457242",
                Release = DateTime.Now,
                Publish = pub,
                User = user
            };      

            int a = db.Insert(user);             
             
            db.Close();

            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestStore2()
        {
            //SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            SqlContext db = new SqlContext(Command.MssqlConnStr);

            db.Depth = 2;

            //db.Create<Order>();

            User user = new User() { Name = "Stephen", BID = 345346.3245d, IsPermit = true, Birthday = DateTime.Now };
   
            Order order = db.Query<Order>().First();

            order.User = user;

            db.Update(order);

            db.Close();

            Assert.IsNotNull(order);
        }

        [TestMethod]
        public void TestStore3()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Depth = 3;

            db.Create<OrderItem>();
            db.Create<Order>();
            db.Create<User>();

            Order order = new Order() { User = new User() { Name = "Peter", BID = 234.564d }, Date = DateTime.Now };

            OrderItem item = new OrderItem() { Name = "Ruler", Order = order, Quantity = 3 };

            int a = db.Insert(item);      

            db.Close();

            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestStore4()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Remove<OrderItem>();
            db.Create<OrderItem>();
                     
            db.Close();

            int a = 1;

            Assert.IsTrue(a > 0);
        }
    }
}
