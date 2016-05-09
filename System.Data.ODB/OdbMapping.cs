using System.Reflection;
using System.Collections.Generic;

namespace System.Data.ODB
{
    public class OdbMapping
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

        public static ColumnAttribute GetColAttribute(PropertyInfo pi)
        {
            object[] objAttrs = pi.GetCustomAttributes(typeof(ColumnAttribute), true);

            if (objAttrs.Length > 0)
            {
                ColumnAttribute colAttr = objAttrs[0] as ColumnAttribute;
                 
                return colAttr;
            }

            return new ColumnAttribute();
        }

        public static IEnumerable<OdbColumn> GetColumn(Type type)
        {
            PropertyInfo[] propes = type.GetProperties();

            for (int i = 0; i < propes.Length; i++)
            {
                ColumnAttribute colAttr = GetColAttribute(propes[i]);

                if (!colAttr.NotMapped)
                {
                    string name = string.IsNullOrEmpty(colAttr.Name) ? propes[i].Name : colAttr.Name;

                    if (colAttr.IsForeignkey && string.IsNullOrEmpty(colAttr.Name))
                    {
                        name = propes[i].Name + "Id";
                    }

                    if (DataType.OdbEntity.IsAssignableFrom(propes[i].PropertyType))
                    {
                        colAttr.IsForeignkey = true;
                    }

                    yield return new OdbColumn(name,  propes[i], colAttr);
                }
            }
        }
    }
}
