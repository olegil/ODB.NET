using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;
using System.Data.ODB.SQLite;

namespace UnitTest
{
    [TestClass]
    public class UnitCUID
    {
        [TestMethod]
        public void TestAdd()
        {
            MyRepository db = new MyRepository();

            db.Clear<User>();
           
            User user = new User() { Name = "Stephen", BID = 123F };
                    
            int a = db.Store(user);
  
            Assert.IsTrue(a > 0);
        }

        [TestMethod]
        public void TestUpdate()
        {
            MyRepository db = new MyRepository();

            User user = db.Collect<User>().First<User>();

            int ret = 0;

            if (user != null)
            {
                user.Birthday = DateTime.Now;

                ret = db.Store(user);
            }
 
            Assert.IsTrue(ret != 0);
        }

        [TestMethod]
        public void TestDelete()
        {
            MyRepository respo = new MyRepository();

            User user = respo.Collect<User>().First<User>();

            IQuery q = respo.Collect<User>().Delete().Where("").Eq(1);

            int a = 0;

            if (user != null)
            {
               

                q.Execute();

                a = respo.Delete(user);
            }
 
            Assert.IsTrue(a > 0);
        }
    }
}