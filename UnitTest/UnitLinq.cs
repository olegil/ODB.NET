﻿using System;
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
            MyRepository respo = new MyRepository();

            var query = from u in respo.Orders
                        where u.User.Name.Length > 3 && u.PackageID.Trim() == "23523145"
                        orderby u.User.Age ascending
                        select u.User;

            var users = query.ToList();

            respo.Dispose();

            Assert.IsTrue(users.Count > 0);
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

            respo.Dispose();

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

            respo.Dispose();

            Assert.IsTrue(list.Count > 0);
        }

        [TestMethod]
        public void TestLingSelect()
        {
            MyRepository respo = new MyRepository();

            var query = respo.Users.Where(p => p.Name.Substring(3, 2) == "Chan");
                        
            //string sql = query.ToString();

            var list = query.ToList();

            respo.Dispose();

            Assert.IsTrue(list.Count > 0);
        }
    }
}
