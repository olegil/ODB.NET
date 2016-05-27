using System;
using System.Data.Common;
using System.Reflection;

namespace System.Data.ODB
{
    public class OdbColumn
    {
        public string Name { get; set; }
        public PropertyInfo Property { get; set; }
        public OdbAttribute Attribute { get; set; }


        public OdbColumn()
        {            
        }    

        public Type GetMapType()
        {
            return this.Property.PropertyType;
        }

        public DbType GetDbType()
        {
            return OdbSqlType.Convert(this.Property.PropertyType);
        }

        public object GetValue(IEntity t)
        {
            return this.Property.GetValue(t, null);
        }

        public void SetValue(IEntity t, object b)
        {
            this.Property.SetValue(t, b, null);            
        }
    }
}
