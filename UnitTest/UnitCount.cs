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
                      
            int n = respo.Count<User>().Where("Name").Eq("Peter").Single(); 
  
            Assert.IsTrue(n > 0);
        }
    }
}
