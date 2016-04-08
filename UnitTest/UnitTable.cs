using System.Text;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitTable
    {
        [TestMethod]
        public void TestTable()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Remove<Book>();

            int a = db.Create<Book>();

            db.Close(); 

            Assert.IsTrue( a > 0);
        }

        [TestMethod]
        public void TestClear()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Clear<User>();

            db.Close();

            int a = 0;

            Assert.IsTrue(a > 0);
        }
    }
}
