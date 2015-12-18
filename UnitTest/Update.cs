using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;

namespace UnitTest
{
    [TestClass]
    public class Update
    {
        [TestMethod]
        public void TestUpdate()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            db.IsEntityTracking = true;

            IQuery<User> q = db.Table<User>();

            User a = q.First();

            int ret = 0;

            if (a != null)
            {
                a.Name = "Good";

                ret = db.Update(a);
            }

            db.Close();

            Assert.IsTrue(ret != 0);
        }
    }
}
