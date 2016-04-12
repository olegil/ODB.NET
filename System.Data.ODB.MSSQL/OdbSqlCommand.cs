using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB.MSSQL
{
    public class OdbSqlCommand : OdbCommand
    {
        public OdbSqlCommand(IDbContext db) : base(db)
        {
        }

        public override void ExecuteCreate<T>()
        {
            Type type = typeof(T);

            this.Create(type);             
        }

        public override void Create(string table, string[] cols)
        {
            string sql = "IF OBJECT_ID('[" + table + "]', 'U') IS NULL CREATE TABLE [" + table + "] (\r\n" + string.Join(",\r\n", cols) + "\r\n);";

            this.Db.ExecuteNonQuery(sql);
        }
 
        public override void Drop(string table)
        {
            string sql = "IF OBJECT_ID('[{0}]', 'U') IS NOT NULL DROP TABLE [{1}];";

            this.Db.ExecuteNonQuery(string.Format(sql, table, table));
        }

        public override int ExecuteInsert(IQuery query)
        {
            string sql = query.ToString();

            int i = sql.IndexOf("VALUES") - 1;

            sql = sql.Insert(i, " OUTPUT INSERTED.Id ");

            return (int)this.Db.ExecuteScalar<long>(sql, query.GetParams());
        } 

        public override string SqlDefine(ColumnMapping col)
        {
            string dbtype = this.SqlMapping(col.Prop.PropertyType);
            string sql = "[" + col.Name + "] " + dbtype;

            if (dbtype == "NVARCHAR")
            {
                if (col.Attribute.Length > 0)
                    sql += "(" + col.Attribute.Length + ")";
                else
                    sql += "(MAX)";
            }

            if (col.Attribute.IsAuto)
            {
                sql += " IDENTITY(1,1)";
            }

            if (col.Attribute.IsPrimaryKey)
            {
                sql += " PRIMARY KEY";
            }

            if (col.Attribute.IsNullable)
            {
                sql += " NULL";
            }
            else
            {
                sql += " NOT NULL";
            }

            return sql;
        }

        public override string SqlMapping(Type type)
        {
            if (type == DataType.String)
            {
                return "NVARCHAR";
            }
            else if (type == DataType.Char)
            {
                return "CHAR(1)";
            }
            else if (type == DataType.SByte)
            {
                return "TINYINT";
            }
            else if (type == DataType.Short || type == DataType.Byte)
            {
                return "SMALLINT";
            }
            else if (type == DataType.Int32 || type == DataType.UShort)
            {
                return "INT";
            }
            else if (type == DataType.Int64 || type == DataType.UInt32)
            {
                return "BIGINT";
            }
            else if (type == DataType.UInt64)
            {
                return "BIGINT";
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
                return "NUMERIC(20,10)";
            }
            else if (type == DataType.Bool)
            {
                return "BIT";
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
                return "GUID";
            }
            else if (DataType.OdbEntity.IsAssignableFrom(type))
            {
                return "INT";
            }

            return "NVARCHAR";
        }
    }
}
