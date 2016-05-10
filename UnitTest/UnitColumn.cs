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
            OdbConfig.Depth = 3;

            OdbDiagram col = new OdbDiagram();

            col.Analyze(typeof(User));

            Assert.IsTrue(col.Columns.Length > 0);
        }
    }
}
