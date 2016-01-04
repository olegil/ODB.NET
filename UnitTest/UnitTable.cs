using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitTable
    {
        [TestMethod]
        public void TestTable()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            db.Remove<User>();
            db.Create<User>();

            User user = new User() { Name = "Stephen", Birthday = DateTime.Parse("1991/8/16") };                   

            db.Insert(user);

            DataSet ds = db.From<User>().Result();
             
            db.Close();

            Assert.IsNotNull(ds);
        }
    }
}
