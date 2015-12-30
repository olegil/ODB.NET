using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;

namespace UnitTest
{
    [TestClass]
    public class UnitUpdate
    {
        [TestMethod]
        public void TestUpdate()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            db.IsEntityTracking = true;

            db.Insert(new User() { Name = "Peter" });

            User user = db.From<User>().First();

            int ret = 0;

            if (user != null)
            {
                user.Name = "ABC";

                ret = db.Update(user);
            }

            db.Close();

            Assert.IsTrue(ret != 0);
        }
    }
}
