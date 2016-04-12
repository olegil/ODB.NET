using System;
using System.Data.Common;
using System.Reflection;

namespace System.Data.ODB
{
    public class ColumnMapping
    {
        public string Name { get; set; }
        public PropertyInfo Prop { get; set; }      
        public ColumnAttribute Attribute { get; private set; }     
  
        public ColumnMapping(string name, PropertyInfo prop, ColumnAttribute attr)
        {            
            this.Name = name;
            this.Prop = prop;           
            this.Attribute = attr;
        }          

        public DbType GetDbType()
        { 
            return SqlType.Convert(this.Prop.PropertyType);
        }

        public object GetValue(IEntity t)
        {
            return this.Prop.GetValue(t, null);
        }
    }
}
