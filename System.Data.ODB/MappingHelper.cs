using System.Reflection;

namespace System.Data.ODB
{
    public class MappingHelper
    {
        public static TableMapping Create<T>(T t) where T : IEntity
        {
            Type type = t.GetType();

            TableMapping table = new TableMapping();

            table.Name = type.Name;

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute attr = GetColumnAttribute(pi);

                if (attr != null)
                {
                    ColumnMapping col = new ColumnMapping(pi.Name, pi.GetValue(t, null), attr);

                    table.Columns.Add(col);

                    if (attr.IsPrimaryKey)
                    {
                        table.PrimaryKey = col;
                    }
                }
            }

            return table;
        }

        public static ColumnAttribute GetColumnAttribute(PropertyInfo pi)
        {
            object[] objAttrs = pi.GetCustomAttributes(typeof(ColumnAttribute), true);

            if (objAttrs.Length > 0)
            {
                return objAttrs[0] as ColumnAttribute;
            }

            return null;
        }       
    }
}
