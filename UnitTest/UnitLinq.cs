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

            var query = from o in db.Orders
                        where o.User.Id == 1
                        orderby o.Id ascending
                        select o;

            var list = query.ToList();
 
            Assert.IsTrue(list.Count > 0);
        }        

        [TestMethod]
        public void TestLinq()
        {
            MyRepository respo = new MyRepository();

            var query = from u in respo.Users
                        where u.Name.Contains("hen") || u.Name.Length > 2 || u.Name.Trim() == "avsd" || u.Name != null && u.Name.ToLower() == "haweg" || u.Name.Equals("gasdhasdh")
                        select u; 

            string sql = query.ToString();

            List<User> list = query.ToList();
 
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestString()
        {
            MyRepository respo = new MyRepository();

            var query = from u in respo.Users
                        where u.Name.IndexOf('C') > 0 || u.Name == "abc"
                        select u;

            string sql = query.ToString();

            List<User> list = query.ToList();
 
            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestLingSelect()
        {
            MyRepository respo = new MyRepository();

            var query = respo.Users.Where(p => p.Name.Substring(3, 2) == "Chan");
                        
            //string sql = query.ToString();

            var list = query.ToList();
 
            Assert.IsTrue(list.Count > 0);
        }
    }
}
