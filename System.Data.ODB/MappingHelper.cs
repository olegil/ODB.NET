using System.Reflection;

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
    }
}
