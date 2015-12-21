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

            IQuery q = db.Select<Book>(new[] { "T1.*" }).As("T1").LeftJoin<User>().As("T2").On("T1.UserId").Equal("T2.Id");

            DataTable dt = db.ExecuteDataSet(q).Tables[0];

            db.Close();

            Assert.IsNotNull(dt);
        }
    }
}
