using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitInsert
    {
        [TestMethod]
        public void TestAdd()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            db.Create<Book>();

            Book book = new Book() { UserId = 1, Release = DateTime.Now };

            int a = db.Insert(book);
           
            db.Close();

            Assert.IsTrue(a > 0);
        }
    }
}