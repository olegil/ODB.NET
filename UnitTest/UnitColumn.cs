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
            OdbTable table = OdbMapping.CreateTable(typeof(User));

            OdbDiagram dg = new OdbDiagram(typeof(User), 1);
            dg.Visit(); 

            string[] cols = dg.Root.GetAllColums();
 
            Assert.IsTrue(cols.Length > 0);
        }

        [TestMethod]
        public void TestJoinTable()
        { 
            OdbTable table = OdbMapping.CreateTable(typeof(User));

            OdbDiagram dg = new OdbDiagram(typeof(User), 2);
            dg.Visit();
        
            string s = dg.Root.GetChilds();

            Assert.IsTrue(s.Length > 0);
        }
    }
}
