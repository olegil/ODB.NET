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

        public static OdbAttribute GetColAttribute(PropertyInfo pi)
        {
            object[] objAttrs = pi.GetCustomAttributes(typeof(OdbAttribute), true);

            if (objAttrs.Length > 0)
            {
                OdbAttribute colAttr = objAttrs[0] as OdbAttribute;
                 
                return colAttr;
            }

            return new OdbAttribute();
        }

        public static OdbTable CreateTable(Type type)
        {
            OdbTable table = new OdbTable(type);

            foreach (OdbColumn col in GetColumns(type))
            {
                table.Columns.Add(col);
            }
            
            return table;
        }

        public static IEnumerable<OdbColumn> GetColumns(Type type)
        {
            PropertyInfo[] propes = type.GetProperties();

            for (int i = 0; i < propes.Length; i++)
            {
                PropertyInfo prop = propes[i];
                OdbAttribute colAttr = GetColAttribute(prop);

                if (!colAttr.IsOmitted)
                {
                    OdbColumn col = new OdbColumn();

                    col.Property = prop;
                    col.Attribute = colAttr;
                    col.Name = string.IsNullOrEmpty(colAttr.Name) ? prop.Name : colAttr.Name;

                    if (prop.Name == "Id")
                    {
                        col.Attribute.IsPrimaryKey = true;
                        col.Attribute.IsAuto = true;
                    }
                    else
                    {
                        if (OdbType.OdbEntity.IsAssignableFrom(prop.PropertyType))
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
