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

            db.SetDepth(2);
             
            string name = "hen";

            var query = from o in db.Orders
                        where o.User.Name.Contains(name)    
                        select o.User;

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
            MyRepository db = new MyRepository();

            string name = "asdf";

            int a = 1;

            User user = new User() { Name = "Peter", Shipping = new Address() { Street = "HK" } };

            var query = from o in db.Orders
                        where o.Remark == name || o.User.Age > a
                        select o; 

            var rs = query.ToList().FirstOrDefault();
 
            Assert.IsNotNull(rs);
        }
    }
}
