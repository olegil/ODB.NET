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

            OdbTable table = OdbMapping.CreateTable(typeof(User));

            OdbDiagram dg = new OdbDiagram(table);
            dg.Visit();

            OdbTree tree = dg.CreateTree();

            string[] cols = tree.GetNodeColumns(table);
 
            Assert.IsTrue(cols.Length > 0);
        }

        [TestMethod]
        public void TestJoinTable()
        {
            OdbConfig.Depth = 3;

            OdbTable table = OdbMapping.CreateTable(typeof(User));

            OdbDiagram dg = new OdbDiagram(table);
            dg.Visit();

            OdbTree tree = dg.CreateTree();

            string s = tree.GetChildNodes(table);

            Assert.IsTrue(s.Length > 0);
        }
    }
}
