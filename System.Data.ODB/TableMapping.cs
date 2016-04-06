using System;
using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB
{
    public class TableMapping
    {
        private IEntity entity;
        public string Name { get; private set; }
       
        public TableMapping(IEntity t)
        {
            this.entity = t;

            this.Name = MappingHelper.GetTableName(this.entity.GetType());
        }

        public IEnumerable<ColumnMapping> GetColumns()
        {
            PropertyInfo[] propes = this.entity.GetType().GetProperties();

            for (int i = 0; i< propes.Length; i++) 
            {
                ColumnAttribute attr = MappingHelper.GetColumnAttribute(propes[i]);
 
                if (!attr.NotMapped)
                {
                    string colName = string.IsNullOrEmpty(attr.Name) ? propes[i].Name : attr.Name;

                    if (attr.IsForeignkey && string.IsNullOrEmpty(attr.Name))
                    {
                        colName = propes[i].Name + "Id";
                    }

                    yield return new ColumnMapping(colName, propes[i].GetValue(this.entity, null), attr);
                }
            }
        }
    }
}
