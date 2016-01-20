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
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            db.Depth = 2;
                       
            IQuery<Book> q = db.Get<Book>();
            
            List<Book> list = q.ToList();
 
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestWhere()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            db.Depth = 2;

            IQuery<User> q = db.Get<User>().Where("Name").Eq("Peter");

            User user = q.First();

            db.Close();

            Assert.IsNotNull(user);
        }

        [TestMethod]
        public void TestFirst()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            db.Depth = 2;

            IQuery<Book> q = db.Get<Book>();

            List<Book> books = q.ToList();

            Assert.IsTrue(books.Count > 0);
        }
    }
}
