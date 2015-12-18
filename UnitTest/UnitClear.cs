using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;

namespace UnitTest
{
    [TestClass]
    public class UnitClear
    {
        [TestMethod]
        public void TestClear()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            Book book = new Book() { UserId = 1 };

            db.Insert(book);

            int a = db.Clear<Book>();

            db.Close();

            Assert.IsTrue(a > 0);
        }
    }
}
