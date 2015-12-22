using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitJoin
    {
        [TestMethod]
        public void TestJoin()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            IQuery<Book> q = db.From<Book>(new[] { "T1.*" }).As("T1").LeftJoin<User>().As("T2").On("T1.UserId = T2.Id");

            DataSet ds  = q.Result();

            db.Close();

            Assert.IsTrue(ds != null);
        }
    }
}
