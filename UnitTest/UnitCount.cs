﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.ODB;
using System.Data.ODB.SQLite;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class UnitCount
    {
        [TestMethod]
        public void TestCount()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            //Book book = new Book() { UserId = 19, Release = DateTime.Now };

            //db.Insert(book);

            long n = db.Count<Book>().Length(); 
 
            db.Close();

            Assert.IsTrue(n > 0);

        }
    }
}
