using System;
using System.Data.Common;
using System.Reflection;

namespace System.Data.ODB
{
    public class ColumnMapping
    {
        public string Name { get; set; }
        public PropertyInfo Property { get; set; }
        public ColumnAttribute Attribute { get; set; }
        
        public ColumnMapping(string name, PropertyInfo prop, ColumnAttribute attr)
        {
            this.Name = name;
            this.Property = prop;
            this.Attribute = attr;
        }    

        public Type GetColumnType()
        {
            return this.Property.PropertyType;
        }

        public DbType GetDbType()
        {
            return SqlType.Convert(this.Property.PropertyType);
        }

        public object GetValue(IEntity t)
        {
            return this.Property.GetValue(t, null);
        }
    }
}
