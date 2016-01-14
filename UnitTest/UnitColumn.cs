﻿using System;
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
            TableSelector col = new TableSelector(3);

            col.Find(typeof(Book));

            Assert.IsTrue(col.Colums.Count > 0);

        }
    }
}
