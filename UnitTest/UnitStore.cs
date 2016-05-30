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
            MyRepository db = new MyRepository();
                        
            db.SetDepth(2);

            Address ship = new Address() { City = "TKO", Street = "Po Lam", Flat = "Metro City" };
            User user = new User() { Balance = 200.00d, Age = 26, Birthday = DateTime.Now, Name = "Joann Chan", IsPermit = true, Shipping = ship };

            Product p = new Product() { BID = "4523462347", Name = "Pencil", Price = 5.00d };

            db.Store(user);
            db.Store(p);

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void TestStore2()
        {
            MyRepository respo = new MyRepository();

            respo.SetDepth(3);

            User user = respo.Collect<User>().First<User>();

            user.ModelState = false;

            Order order = new Order() { PackageID = "832578-w7",  User = user, Date = DateTime.Now };

            Product p = respo.Collect<Product>().Where("name").Eq("Pencil").First<Product>();

            OrderItem item = new OrderItem() { Order = order, Item = p, Quantity = 2, CreateDate = DateTime.Now };
      
            respo.Store(item);      
 
            Assert.IsTrue(user.Id > 0);
        }

        [TestMethod]
        public void TestStore3()
        {
            MyRepository respo = new MyRepository();

            respo.SetDepth(2);

            User user = respo.Collect<User>().First<User>();

            user.Shipping.ModelState = false;

            respo.Store(user);

            Assert.IsNotNull(user);
        }
    }
}
