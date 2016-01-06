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
             
            int a = db.Clear<User>();

            db.Close();

            Assert.IsTrue(a > 0);
        }
    }
}
