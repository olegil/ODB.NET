using System.Collections.Generic;
using System.Reflection;
using System.Data.SqlClient;

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

            return (int)this.Db.ExecuteScalar<long>(sql, query.Parameters.ToArray());
        } 

        public override string SqlDefine(ColumnMapping col)
        {
            string dbtype = this.TypeMapping(col.GetDbType());
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

        public override string TypeMapping(DbType type)
        {
            if (type == DbType.String)
            {
                return "NVARCHAR";
            }
            else if (type == DbType.StringFixedLength)
            {
                return "CHAR(1)";
            }
            else if (type == DbType.SByte)
            {
                return "TINYINT";
            }
            else if (type == DbType.Int16 || type == DbType.Byte)
            {
                return "SMALLINT";
            }
            else if (type == DbType.Int32 || type == DbType.UInt16)
            {
                return "INT";
            }
            else if (type == DbType.Int64 || type == DbType.UInt32)
            {
                return "BIGINT";
            }           
            else if (type == DbType.Double)
            {
                return "FLOAT";
            }
            else if (type == DbType.Single)
            {
                return "REAL";
            }
            else if (type == DbType.Decimal)
            {
                return "NUMERIC(20,10)";
            }
            else if (type == DbType.Boolean)
            {
                return "BIT";
            }
            else if (type == DbType.DateTime)
            {
                return "DATETIME";
            }
            else if (type == DbType.Binary)
            {
                return "BINARY";
            }
            else if (type == DbType.Guid)
            {
                return "GUID";
            } 

            return "NVARCHAR";
        }

        public override IDbDataParameter CreateParameter()
        {
            return new SqlParameter();
        }
    }
}
