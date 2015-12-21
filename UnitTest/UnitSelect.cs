using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;

namespace UnitTest
{
    [TestClass]
    public class UnitSelect
    {
        [TestMethod]
        public void TestSelect()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            IQuery<Book> q = db.Select<Book>(new[] { "Id", "UserId" }).Where("Id").Eq(1).And("UserId").Eq(2);

            DataTable dt = db.ExecuteDataSet(q).Tables[0];

            Assert.IsNotNull(dt);
        }
    }
}
