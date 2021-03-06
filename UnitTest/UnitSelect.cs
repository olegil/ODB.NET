﻿using System;
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
 
            IQuery q = respo.Collect<User>();

            User user = q.First<User>();

            Assert.IsNotNull(user);
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
            MyRepository db = new MyRepository();

            db.SetDepth(3);

            OrderItem order = db.Collect<OrderItem>().First<OrderItem>(); 

            Assert.IsTrue(order != null);
        }

        [TestMethod]
        public void TestLike()
        { 
            MyRepository db = new MyRepository();

            db.SetDepth(2);

            IQuery q = db.Collect<User>().Where("Name").Like("Peter");

            List<User> list = q.ToList<User>();
             
            Assert.IsTrue(list.Count > 0);
        }
    }
}
