using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;

namespace UnitTest
{
    [TestClass]
    public class UnitCUID
    {
        [TestMethod]
        public void TestAdd()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Clear<User>();
           
            User user = new User() { Name = "Stephen", BID = 123F };
                    
            int a = db.Insert(user);
 
            db.Close();

            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestUpdate()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            User user = db.Query<User>().First();

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
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));
                     
            User user = db.Query<User>().First();

            IQuery<User> q =  db.CreateQuery<User>().Delete().Where("").Eq(1);

            int a = 0;

            if (user != null)
            {
               

                q.Execute();

                a = db.Delete(user);
            }

            db.Close();

            Assert.IsTrue(a > 0);
        }
    }
}