﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB.SQLite;

namespace UnitTest
{
    [TestClass]
    public class UnitInsert
    {
        [TestMethod]
        public void TestAdd()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            //db.Clear<User>();
           // db.Clear<Address>();

            User user = new User() { Name = "Peter", Birthday = DateTime.Now, Address = new Address() { Flat = "64", Street = "Queen Road", City = "HK" } };
                    
            int a = db.Insert(user);
 
            db.Close();

            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestUpdate()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            //db.IsEntityTracking = true; 

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

        [TestMethod]
        public void TestDelete()
        {
            SQLiteContext db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            //db.Create<User>();

            //db.Insert(new User() { Name = "Peter" });

            User user = db.From<User>().First();

            int a = 0;

            if (user != null)
            {
                a = db.Delete(user);
            }

            db.Close();

            Assert.IsTrue(a > 0);
        }
    }
}