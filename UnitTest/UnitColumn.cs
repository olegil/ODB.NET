using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.ODB;

namespace UnitTest
{
    [TestClass]
    public class UnitColumn
    {
        [TestMethod]
        public void TestColumns()
        { 
            OdbDiagram col = new OdbDiagram(3);

            col.Visit(typeof(User), 0);

            Assert.IsTrue(col.Colums.Length > 0);
        }
    }
}
