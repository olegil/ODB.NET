﻿using System;
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
            SqlContext db = new SqlContext(Command.MssqlConnStr);

            db.Depth = 2;

            db.Create<Book>();

            Publish pub = new Publish() { Name = "Bloger", Address = new Address() { Flat = "64", Street = "ABC", City = "Hong Kong" } };
            User user = new User() { Name = "Peter", BID = null, Birthday = DateTime.Now };

            Book book = new Book()
            {
                ISBN = "JTSEWAGEASg-3457242",
                Release = DateTime.Now,
                Publish = pub,
                User = user
            };      

            int a = db.Insert(user);

            IQuery<Order> q = db.Query<Order>().Where("Id").Eq(1);

            List<Order> oo = q.ToList();

            db.Close();

            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestStore2()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Depth = 2;

            Order oo = new Order() { User = new User() { Name = "Peter", BID = "534262346", Birthday = DateTime.Now }, Date = DateTime.Now };

            db.Insert(oo);

            IQuery<User> q1 = db.Query<User>().Where("Name").Eq("Peter");

            IQuery<Order> q = db.Query<Order>().Where("Id").Eq(1);

            Order order = q.First();

            int a = 0;

            if (order != null)
            {
                order.User.Name = "Ken";

                a = db.Update(order);
            }

            db.Close();

            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestStore3()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Depth = 3;

            db.Create<OrderItem>();
            db.Create<Order>();
            db.Create<User>();

            Order order = new Order() { User = new User() { Name = "Peter", BID = "7277-7257257" }, Date = DateTime.Now };

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
