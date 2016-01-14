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

            Book book = new Book() { ISBN = "RAYAWEVQhg-3464528", User = new User() { Name = "Joanne", Birthday = DateTime.Now, Address = new Address() { City = "HK", Flat = "7", Street = "Kings Road" } }, Release = DateTime.Now };

            db.Store(book);

            IQuery<Book> q = db.Get<Book>();
            
            List<Book> list = q.ToList();
 
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestWhere()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));
             
            IQuery<Book> q = db.Get<Book>().Where("Id").Gt(0);

            Book book = q.First();

            db.Close();

            Assert.IsNotNull(book);
        }
    }
}
