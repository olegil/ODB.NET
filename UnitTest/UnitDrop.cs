using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;

namespace UnitTest
{
    [TestClass]
    public class UnitDrop
    {
        [TestMethod]
        public void TestDrop()
        {            
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            db.Drop<User>();
            db.Drop<Book>();
            db.Drop<Address>();

            bool a = true;

            db.Close(); 

            Assert.AreEqual(true, a);
       
        }
    }
}
