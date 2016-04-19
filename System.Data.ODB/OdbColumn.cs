﻿using System;
using System.Data.Common;
using System.Reflection;

namespace System.Data.ODB
{
    public class OdbColumn
    {
        public string Name { get; private set; }
        public PropertyInfo Property { get; private set; }
        public ColumnAttribute Attribute { get; private set; }
        
        public OdbColumn(string name, PropertyInfo prop, ColumnAttribute attr)
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
            return OdbSqlType.Convert(this.Property.PropertyType);
        }

        public object GetValue(IEntity t)
        {
            return this.Property.GetValue(t, null);
        }
    }
}