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

            IQuery q = db.Select<Book>().Where("Id").Gt(0);

            List<Book> books = q.ToList<Book>();

            db.Close();

            Assert.IsTrue(books.Count == 2);
        }
    }
}
