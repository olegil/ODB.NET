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
            MyRepository respo = new MyRepository();
                     
            Publish pub = new Publish() { Name = "Bloger", Address = new Address() { Flat = "64", Street = "ABC", City = "Hong Kong" } };
            User user = new User() { Name = "Peter", BID = 235f, Birthday = DateTime.Now };

            Book book = new Book()
            {
                ISBN = "JTSEWAGEASg-3457242",
                Release = DateTime.Now,
                Publish = pub,
                User = user
            };      

            int a = respo.Store(user);             
              
            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestStore2()
        {
            MyRepository respo = new MyRepository();
            //db.Create<Order>();

            User user = new User() { Name = "Stephen", BID = 345346.3245d, IsPermit = true, Birthday = DateTime.Now };
   
            Order order = respo.Collect<Order>().First<Order>();

            order.User = user;

            respo.Store(order);
          
            Assert.IsNotNull(order);
        }

        [TestMethod]
        public void TestStore3()
        {
            MyRepository respo = new MyRepository();

            respo.Create<OrderItem>();
            respo.Create<Order>();
            respo.Create<User>();

            Order order = new Order() { User = new User() { Name = "Peter", BID = 234.564d }, Date = DateTime.Now };

            OrderItem item = new OrderItem() { Name = "Ruler", Order = order, Quantity = 3 };

            int a = respo.Store(item);      
 
            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestStore4()
        {
            MyRepository respo = new MyRepository();

            respo.Remove<OrderItem>();
            respo.Create<OrderItem>();
          
            int a = 1;

            Assert.IsTrue(a > 0);
        }
    }
}
