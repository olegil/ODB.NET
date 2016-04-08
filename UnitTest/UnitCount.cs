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
            SQLiteContext db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            Book book = new Book() {  ISBN = "ANDSAGSDAH", Release = DateTime.Now, User = new User() { BID = "", Name = "Chan Peter" } };

            db.Insert(book);

            int n = db.Count<Book>(); 
 
            db.Close();

            Assert.IsTrue(n > 0);

        }
    }
}
