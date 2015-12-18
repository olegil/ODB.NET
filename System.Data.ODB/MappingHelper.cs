using System; 
using System.Reflection;

namespace System.Data.ODB
{
    public class MappingHelper
    {
        public static TableMapping Create<T>(T t) where T : IEntity
        {
            Type type = t.GetType();

            TableMapping table = new TableMapping();

            table.Name = GetTableName(type);

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

        public static string GetTableName(Type type)
        {
            object[] objAttrs = type.GetCustomAttributes(typeof(TableAttribute), false);

            if (objAttrs.Length > 0)
            {
                TableAttribute tableAttribute = objAttrs[0] as TableAttribute;

                if (tableAttribute != null && !string.IsNullOrEmpty(tableAttribute.Name))
                {
                    return tableAttribute.Name;
                }
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

            return null;
        }

        public static DbType TypeConvert(object obj)
        {
            if (obj is string)
            {
                return DbType.String;
            }
            else if (obj is char)
            {
                return DbType.StringFixedLength;
            }
            else if (obj is byte)
            {
                return DbType.Byte;
            }
            else if (obj is byte[])
            {
                return DbType.Binary;
            }
            else if (obj is sbyte)
            {
                return DbType.SByte;
            }
            else if (obj is int)
            {
                return DbType.Int32;
            }
            else if (obj is uint)
            {
                return DbType.UInt32;
            }
            else if (obj is short)
            {
                return DbType.Int16;
            }
            else if (obj is ushort)
            {
                return DbType.UInt16;
            }
            else if (obj is long)
            {
                return DbType.Int64;
            }
            else if (obj is ulong)
            {
                return DbType.UInt64;
            }
            else if (obj is double)
            {
                return DbType.Double;
            }
            else if (obj is float)
            {
                return DbType.Single;
            }
            else if (obj is decimal)
            {
                return DbType.Decimal;
            }
            else if (obj is bool)
            {
                return DbType.Boolean;
            }
            else if (obj is DateTime)
            {
                return DbType.DateTime;
            }
            else if (obj is Guid)
            {
                return DbType.Guid;
            }
           
            return DbType.String; 
        }

        public static string DataConvert(Type type)
        {
            if (type == DataType.String)
            {
                return "TEXT";
            }
            else if (type == DataType.Char)
            {
                return "VARCHAR(50)";
            }
            else if (type == DataType.Byte)
            {
                return "TINYINT";
            }
            else if (type == DataType.SByte)
            {
                return "TINYINT";
            }
            else if (type == DataType.Interger)
            {
                return "INTEGER";
            }
            else if (type == DataType.UInt)
            {
                return "INTEGER";
            }
            else if (type == DataType.Short)
            {
                return "SMALLINT";
            }
            else if (type == DataType.UShort)
            {
                return "SMALLINT";
            }
            else if (type == DataType.Long)
            {
                return "INTEGER";
            }
            else if (type == DataType.ULong)
            {
                return "INTEGER";
            }
            else if (type == DataType.Double)
            {
                return "DOUBLE";
            }
            else if (type == DataType.Float)
            {
                return "REAL";
            }
            else if (type == DataType.Decimal)
            {
                return "NUMERIC";
            }
            else if (type == DataType.Bool)
            {
                return "BOOLEAN";
            }
            else if (type == DataType.DateTime)
            {
                return "DATETIME";
            }
            else if (type == DataType.Bytes)
            {
                return "BLOB";
            }           
            else if (type == DataType.Guid)
            {
                return "VARCHAR(50)";
            }

            return "TEXT";
        }
    }
}
