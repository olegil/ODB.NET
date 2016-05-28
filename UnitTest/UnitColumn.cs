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
            Type type = typeof(User);

            OdbDiagram dg = new OdbDiagram(1);
            dg.CreateTableList(typeof(User));

            string[] cols = dg.GetColumns(type);
 
            Assert.IsTrue(cols.Length > 0);
        }

        [TestMethod]
        public void TestJoinTable()
        {
            Type type = typeof(User);

            OdbDiagram dg = new OdbDiagram(2);
            dg.CreateTableList(typeof(User));

            string[] cols = dg.GetColumns(type);

            Assert.IsTrue(cols.Length > 0);
        }
    }
}
