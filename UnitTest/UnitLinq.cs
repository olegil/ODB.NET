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
            MyRepository db = new MyRepository();

            int a = 1;
            string name = "hen";

            var query = from o in db.Orders
                        where o.User.Name.Contains("stephen")    
                        select o;

            var list = query.ToList();

            Assert.IsTrue(list.Count > 0);
        }        

        [TestMethod]
        public void TestString()
        {           
            MyRepository db = new MyRepository();

            db.SetDepth(2);

            var query = from u in db.Users
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

            var user = query.First();
 
            Assert.IsNotNull(user);
        }
    }
}
