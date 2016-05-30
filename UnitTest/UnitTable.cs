using System; 
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
 
namespace UnitTest
{
    [TestClass]
    public class UnitTable
    {
        [TestMethod]
        public void TestCreate()
        {
            MyRepository respo = new MyRepository();
                       
            respo.Create<User>();
        
            int a = 1;
            Assert.IsTrue(a == 1);        
        }

        [TestMethod]
        public void TestRemove()
        {
            MyRepository respo = new MyRepository();

           // respo.Remove<OrderItem>();
           // respo.Remove<Order>();
            respo.Remove<User>();
           // respo.Remove<Address>();

            int a = 1;
            Assert.IsTrue(a == 1);
        }
    }
}
