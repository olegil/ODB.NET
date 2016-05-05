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

            IQuery q = respo.Collect<Order>();

            List<Order> list = q.ToList<Order>();
  
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestWhere()
        {
            MyRepository db = new MyRepository();

            User user = new User() { Name = "Peter", BID = 345f, Birthday = DateTime.Now };

            db.Store(user);

            IQuery q = db.Collect<User>().Where("Name").Eq("Peter");
            
            User user2 = q.First<User>();
 
            Assert.IsNotNull(user);
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
            //SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            MyRepository respo = new MyRepository();

            IQuery q = respo.Collect<User>().Where("Name").Like("hen");

            List<User> list = q.ToList<User>();
             
            Assert.IsTrue(list.Count > 0);
        }
    }
}
