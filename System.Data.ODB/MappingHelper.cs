using System.Reflection;
using System.Collections.Generic;

namespace System.Data.ODB
{
    public class MappingHelper
    {
        public static string GetTableName(Type type)
        {
            object[] tableAttributes = type.GetCustomAttributes(typeof(TableAttribute), false);
 
            if (tableAttributes.Length > 0)
            {
                TableAttribute attribute = tableAttributes[0] as TableAttribute;

                return attribute.Name;
            }

            return type.Name;
        }

        public static ColumnAttribute GetColumnAttribute(PropertyInfo pi)
        {
            object[] objAttrs = pi.GetCustomAttributes(typeof(ColumnAttribute), true);

            if (objAttrs.Length > 0)
            {
                return objAttrs[0] as ColumnAttribute;
            }

            return new ColumnAttribute() { Name = "", IsAuto = false, IsForeignkey = false, IsPrimaryKey = false, IsNullable = true, NotMapped = false, Length = 0 };
        }

        public static IEnumerable<ColumnMapping> GetColumnMapping(IEntity t)
        {
            PropertyInfo[] propes = t.GetType().GetProperties();

            for (int i = 0; i < propes.Length; i++)
            {
                ColumnAttribute attr = MappingHelper.GetColumnAttribute(propes[i]);

                if (!attr.NotMapped)
                {
                    string colName = string.IsNullOrEmpty(attr.Name) ? propes[i].Name : attr.Name;

                    if (attr.IsForeignkey && string.IsNullOrEmpty(attr.Name))
                    {
                        colName = propes[i].Name + "Id";
                    }

                    yield return new ColumnMapping(colName, propes[i].GetValue(t, null), attr);
                }
            }
        }
    }
}
