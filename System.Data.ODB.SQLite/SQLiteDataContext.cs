using System.Collections.Generic;
using System.Reflection;
using System.Data.SQLite;

namespace System.Data.ODB.SQLite
{
    public class SQLiteDataContext : OdbContext
    {
        public SQLiteDataContext(IDbConnection conn) : base(conn)
        {
        }

        public override IQuery Query()
        {
            return new SQLiteQuery(this);
        }

        public override void Create(string table, string[] cols)
        {
            string sql = "CREATE TABLE IF NOT EXISTS [" + table + "] (\r\n" + string.Join(",\r\n", cols) + "\r\n);";

            this.ExecuteNonQuery(sql);
        }

        public override IDbDataParameter CreateParameter()
        {             
            return new SQLiteParameter();
        }

        public override void Drop(string table)
        {
            this.ExecuteNonQuery("DROP TABLE IF EXISTS [" + table + "]");
        }
        
        public override string SqlDefine(OdbColumn col)
        {
            string dbtype = this.TypeMapping(col.GetDbType());
            string sql = "[" + col.Name + "]";

            ColumnAttribute attr = col.Attribute;

            if (attr.IsKey)
            {
                sql += " INTEGER PRIMARY KEY";
            }
            else
            {
                sql += dbtype;

            }

            if (attr.IsAuto)
            {
                sql += " AUTOINCREMENT";
            }

            if (attr.IsNullable && !attr.IsKey)
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
                return "TEXT";
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
                return "INTEGER";
            }         
            else if (type == DbType.Double)
            {
                return "REAL";
            }
            else if (type == DbType.Single)
            {
                return "FLOAT";
            }
            else if (type == DbType.Decimal)
            {
                return "NUMERIC(20,10)";
            }
            else if (type == DbType.Boolean)
            {
                return "BOOLEAN";
            }
            else if (type == DbType.DateTime)
            {
                return "TIMESTAMP";
            }
            else if (type == DbType.Binary)
            {
                return "BLOB";
            }
            else if (type == DbType.Guid)
            {
                return "GUID";
            }
         
            return "TEXT";
        }        
    }
}
