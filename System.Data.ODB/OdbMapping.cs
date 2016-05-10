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
                PropertyInfo prop = propes[i];
                ColumnAttribute colAttr = GetColAttribute(prop);

                if (!colAttr.NotMapped)
                {
                    OdbColumn col = new OdbColumn();

                    col.Property = prop;
                    col.Attribute = colAttr;
                    col.Name = string.IsNullOrEmpty(colAttr.Name) ? prop.Name : colAttr.Name;

                    if (prop.Name == "Id")
                    {
                        col.IsPrimaryKey = true;
                    }
                    else
                    {
                        if (DataType.OdbEntity.IsAssignableFrom(prop.PropertyType))
                        {
                            col.IsForeignkey = true;

                            if (string.IsNullOrEmpty(colAttr.Name))
                                col.Name += "Id";
                        }
                    }               

                    yield return col;
                }
            }
        }
    }
}
