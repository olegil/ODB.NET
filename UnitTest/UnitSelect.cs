using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Data.ODB.MSSQL;

namespace UnitTest
{
    [TestClass]
    public class UnitSelect
    {
        [TestMethod]
        public void TestSelect()
        {
            MyRepository respo = new MyRepository();
 
            List<Order> list = respo.Collect<Order>().ToList<Order>();
  
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestWhere()
        {
            MyRepository db = new MyRepository();

            User user = new User() { Name = "Peter", Balance = 345f, Birthday = DateTime.Now };

            db.Store(user);

            IQuery q = db.Collect<User>().Where("Name").Eq("Peter");
            
            List<User> users = q.ToList<User>();
 
            Assert.IsTrue(users.Count > 0);
        }

        [TestMethod]
        public void TestFirst()
        {
            MyRepository respo = new MyRepository();

            User user = respo.Collect<User>().First<User>();

            Assert.IsTrue(user != null);
        }

        [TestMethod]
        public void TestLike()
        { 
            MyRepository respo = new MyRepository();

            IQuery q = respo.Collect<User>().Where("Name").Like("Peter");

            List<User> list = q.ToList<User>();
             
            Assert.IsTrue(list.Count > 0);
        }
    }
}
