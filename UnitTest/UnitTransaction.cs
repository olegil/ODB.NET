using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;

namespace UnitTest
{
    [TestClass]
    public class UnitTransaction
    {
        [TestMethod]
        public void TestBatch()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            db.Create<User>();

            db.StartTrans();

            for(int i = 0; i < 10; i++)
            {
                User user = new User() { Name = "Chan", BID = 645.75f };

                db.Insert(user);
            }

            db.CommitTrans();

            db.Close();        
        }
    }
}
