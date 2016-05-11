using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.ODB;
using System.Data.ODB.Linq;

namespace UnitTest
{
    [TestClass]
    public class UnitLinq
    {         
        [TestMethod]
        public void TestExpression()
        {
            OdbConfig.Depth = 3;

            MyRepository db = new MyRepository();

            var query = from o in db.Orders
                        where o.Id == 1                        
                        select o.User;

            string sql = query.ToString(); 
 
            Assert.IsTrue(sql.Length > 0);
        }        

        [TestMethod]
        public void TestString()
        {
            OdbConfig.Depth = 2;
            MyRepository respo = new MyRepository();

            var query = from u in respo.Users
                        where u.Name.Contains("hen") || u.Name.Length > 2 || u.Name.Trim() == "avsd" || u.Name != null && u.Name.ToLower() == "haweg" || u.Name.Equals("gasdhasdh")
                        select u.Shipping; 

            string sql = query.ToString();

            var list = query.ToList();

            Assert.IsNotNull(list);
        } 

        [TestMethod]
        public void TestLingList()
        {
            MyRepository respo = new MyRepository();

            var query = respo.Users.Where(p => p.Name.Substring(3, 2) == "Chan");
                        
            //string sql = query.ToString();

            var list = query.ToList();
 
            Assert.IsTrue(list.Count > 0);
        }
    }
}
