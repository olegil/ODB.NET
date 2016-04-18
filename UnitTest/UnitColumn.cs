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

            col.Analyze(typeof(User));

            Assert.IsTrue(col.Colums.Length > 0);
        }
    }
}
