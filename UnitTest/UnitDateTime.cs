using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.ODB;
using System.Data.ODB.Linq;

namespace UnitTest
{
    [TestClass]
    public class UnitDateTime
    {
        [TestMethod]
        public void TestDateTime()
        {
            MyRepository respo = new MyRepository();

            var query = from u in respo.Users
                        where u.Birthday <= DateTime.Now
                        select u;

            string sql = query.ToString();

            List<User> list = query.ToList();

            respo.Dispose();

            Assert.IsTrue(list.Count > 0);
        }
    }
}
