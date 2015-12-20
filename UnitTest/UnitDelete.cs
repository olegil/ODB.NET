using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;

namespace UnitTest
{
    [TestClass]
    public class UnitDelete
    {
        [TestMethod]
        public void TestDelete()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            //db.Create<User>();
 
            db.Insert(new User() { Name = "Peter" });

            User user = db.Table<User>().First();
                      
            int a = 0;

            if (user != null)
            {
                a = db.Delete(user);
            }

            db.Close();

            Assert.IsTrue(a > 0);            
        }
    }
}
