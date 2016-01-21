using System;
using System.Data.Common;

namespace System.Data.ODB
{
    public class ColumnMapping
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ColumnAttribute Attribute { get; private set; }     
  
        public ColumnMapping(string name, object val, ColumnAttribute attr)
        {            
            this.Name = name;
            this.Value = val;
            this.Attribute = attr;
        }          
    }
}
