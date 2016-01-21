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
            TableVisitor col = new TableVisitor(3);

            col.Visit(typeof(Book), 0);

            Assert.IsTrue(col.Colums.Length > 0);
        }
    }
}
