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

            db.Create<User>();

            User user = new User() { Name = "Peter" };

            db.Insert(user);

            IQuery<User> q = db.Table<User>();

            User user_f = q.First();

            int a = 0;

            if (user_f != null)
            {
                a = db.Delete(user_f);
            }

            db.Close();

            Assert.IsTrue(a > 0);            
        }
    }
}
