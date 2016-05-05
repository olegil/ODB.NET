using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.ODB;
using System.Data.ODB.Linq;

namespace UnitTest
{
    [TestClass]
    public class UnitOrderBy
    {
        [TestMethod]
        public void TestOrder()
        {
            MyRepository respo = new MyRepository();

            var query = from u in respo.Users                         
                        orderby u.Id, u.Name                              
                        select u;

            string sql = query.ToString();

            List<User> list = query.ToList();
 
            Assert.IsTrue(list.Count > 0);
        }
    }
}
