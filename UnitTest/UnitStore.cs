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
        public void TestStore1()
        {
            OdbConfig.Depth = 2;

            MyRepository db = new MyRepository();

            Address ship = new Address() { City = "TKO", Street = "Po Lam", Flat = "Metro City" };
            User user = new User() { Balance = 200.00d, Age = 26, Birthday = DateTime.Now, Name = "Stephen Ieong", IsPermit = true, Shipping = ship };

            Product p = new Product() { BID = "4523462347", Name = "Pencil", Price = 5.00d };

            db.Store(user);
            db.Store(p);

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void TestStore2()
        {
            MyRepository respo = new MyRepository();

            OdbConfig.Depth = 3;

            User user = respo.Collect<User>().First<User>();

            Product p = respo.Collect<Product>().Where("name").Eq("Pencil").First<Product>();

            Order order = new Order() { PackageID = "23523145",  User = user, Date = DateTime.Now };

            OrderItem item = new OrderItem() { Order = order, Item = p, Quantity = 2, CreateDate = DateTime.Now };
      
            respo.Store(item);      
 
            Assert.IsTrue(user.Id > 0);
        }

        [TestMethod]
        public void TestStore3()
        {
            MyRepository respo = new MyRepository();

            OdbConfig.Depth = 2;

            OrderItem o = respo.Collect<OrderItem>().First<OrderItem>();
 
            Assert.IsNotNull(o.Order);
        }

    }
}
