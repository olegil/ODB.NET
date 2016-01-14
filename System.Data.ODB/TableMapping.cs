using System;
using System.Collections.Generic;

namespace System.Data.ODB
{
    public class TableMapping
    {
        public string Name { get; set; }
        public IList<ColumnMapping> Columns { get; private set; }

        public ColumnMapping PrimaryKey { get; set; }
 
        public TableMapping(string name = "")
        {
            this.Name = name;
            this.Columns = new List<ColumnMapping>();
        }         
    }
}
