using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB.SQLite;

namespace UnitTest
{
    [TestClass]
    public class UnitCUID
    {
        [TestMethod]
        public void TestAdd()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            db.Clear<User>();
           
            User user = new User() { Name = "Stephen", BID = "3B5A2DA0-9A4D-4093-9702-51DB611F7B33" };
                    
            int a = db.Insert(user);
 
            db.Close();

            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestUpdate()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            User user = db.Select<User>().First();

            int ret = 0;

            if (user != null)
            {
                user.Birthday = DateTime.Now;

                ret = db.Update(user);
            }

            db.Close();

            Assert.IsTrue(ret != 0);
        }

        [TestMethod]
        public void TestDelete()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));
                     
            User user = db.Select<User>().First();

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