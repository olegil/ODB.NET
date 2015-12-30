using System.Data;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;

namespace UnitTest
{
    [TestClass]
    public class UnitWhere
    {
        [TestMethod]
        public void TestWhere()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            IQuery<Book> q = db.From<Book>().Where("Id").Gt(0);

            List<Book> books = q.ToList();

            db.Close();

            Assert.IsTrue(books.Count > 0);
        }
    }
}
