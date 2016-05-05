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
    public class UnitCount
    {
        [TestMethod]
        public void TestCount()
        {
            MyRepository respo = new MyRepository();

            User user = new User() { BID = 2345F, Name = "Chan Peter" };

            respo.Store(user);

            int n = 0; // respo.Count<User>(); 
  
            Assert.IsTrue(n > 0);

        }
    }
}
