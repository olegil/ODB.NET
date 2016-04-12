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
                       
            IQuery<Book> q = db.Query<Book>();

            List<Book> list = q.ToList();

            string sql = q.ToString();

            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestWhere()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Depth = 2;

            IQuery<User> q = db.Query<User>().Where("Name").Eq("Peter");
            
            User user = q.First();

            db.Close();

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void TestFirst()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Depth = 2;

            IQuery<User> q = db.Query<User>();

            User user = q.First();

            Assert.IsTrue(user != null);
        }

        [TestMethod]
        public void TestQuery()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Clear<User>();

            User user = new User() { Name = "Peter", BID = "2315-34436-346", Birthday = DateTime.Now };

            db.Insert(user);

            user = db.Query<User>().First();
             
            Assert.IsTrue(user != null);
        }
    }
}
