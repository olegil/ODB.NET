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

            Book book = new Book() { ISBN = "asdfaghsd", User = new User() { Name = "Joanne", Birthday = DateTime.Now, Address = new Address() { City = "HK", Flat = "7", Street = "Kings Road" } }, Release = DateTime.Now };

            db.Store(book);

            IQuery<Book> q = db.Table<Book>();
            
            Book b = q.First();
 
            Assert.IsTrue(b != null);
        }

        [TestMethod]
        public void TestWhere()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));
             
            IQuery<Book> q = db.Table<Book>().Where("Id").Gt(0);

            List<Book> list = q.ToList();

            db.Close();

            Assert.IsTrue(list.Count > 0);
        }
    }
}
