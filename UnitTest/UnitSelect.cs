using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;

namespace UnitTest
{
    [TestClass]
    public class UnitSelect
    {
        [TestMethod]
        public void TestSelect()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Depth = 2;
                       
            IQuery<Order>  q = db.Query<Order>();

            List<Order> list = q.ToList();
  
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestWhere()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Depth = 2;

            User user = new User() { Name = "Peter", BID = 345f, Birthday = DateTime.Now };

            db.Insert(user);

            IQuery<User> q = db.Query<User>().Where("Name").Eq("Peter");
            
            User user2 = q.First();

            db.Close();

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void TestFirst()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Depth = 2;
  
            User user = db.Query<User>().First();

            Assert.IsTrue(user != null);
        }

        [TestMethod]
        public void TestQuery()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Clear<User>();

            User user = new User() { Name = "Stephen", BID = 345f, Birthday = DateTime.Now };

            db.Insert(user);

            user = db.Query<User>().First();
             
            Assert.IsTrue(user != null);
        }
    }
}
