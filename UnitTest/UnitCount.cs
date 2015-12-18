using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitCount
    {
        [TestMethod]
        public void TestCount()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            Book book = new Book() { UserId = 19, Release = DateTime.Now };

            db.Insert(book);

            IQuery<Book> q = db.Count<Book>();

            long n = db.ExecuteScalar<Int64>(q.ToString());

            db.Close();

            Assert.IsTrue(n > 0);

        }
    }
}
