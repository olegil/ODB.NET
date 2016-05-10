using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest
{
    [TestClass]
    public class UnitClear
    {
        [TestMethod]
        public void TestClear()
        {
            MyRepository db = new MyRepository();

            db.Clear<Order>();
            db.Clear<OrderItem>();
            db.Clear<Address>();

            db.Clear<User>();
            db.Clear<Product>();

            int a = 0;

            Assert.IsTrue(a > 0);
        }
    }
}
