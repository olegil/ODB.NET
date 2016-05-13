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

        public static OdbTable CreateTable(Type type)
        {
            OdbTable table = new OdbTable(type);

            foreach (OdbColumn col in GetColumn(type))
            {
                table.Columns.Add(col);
            }
            
            return table;
        }

        public static IEnumerable<OdbColumn> GetColumn(Type type)
        {
            PropertyInfo[] propes = type.GetProperties();

            for (int i = 0; i < propes.Length; i++)
            {
                PropertyInfo prop = propes[i];
                ColumnAttribute colAttr = GetColAttribute(prop);

                if (!colAttr.IsOmitted)
                {
                    OdbColumn col = new OdbColumn();

                    col.Property = prop;
                    col.Attribute = colAttr;
                    col.Name = string.IsNullOrEmpty(colAttr.Name) ? prop.Name : colAttr.Name;

                    if (prop.Name == "Id")
                    {
                        col.Attribute.IsKey = true;
                        col.Attribute.IsAuto = true;
                    }
                    else
                    {
                        if (DataType.OdbEntity.IsAssignableFrom(prop.PropertyType))
                        {
                            col.Attribute.IsModel = true;

                            //class_name_Id
                            if (string.IsNullOrEmpty(colAttr.Name))
                                col.Name = prop.PropertyType.Name + "Id";
                        }                       
                    }               

                    yield return col;
                }
            }
        } 
    }
}
