using System.Collections.Generic;
using System.Reflection;
using System.Data.SQLite;

namespace System.Data.ODB.SQLite
{
    public class OdbLiteCommand : OdbCommand
    {
        public OdbLiteCommand(IDbContext db) : base(db)
        {
        }
         
        public override void Create(string table, string[] cols)
        {
            string sql = "CREATE TABLE IF NOT EXISTS [" + table + "] (\r\n" + string.Join(",\r\n", cols) + "\r\n);";

            this.Db.ExecuteNonQuery(sql);
        }

        public override void Drop(string table)
        {
            this.Db.ExecuteNonQuery("DROP TABLE IF EXISTS [" + table + "]");
        }

        public override int ExecuteInsert(IQuery query)
        {
            this.Execute(query);

            return (int)(this.Db.Connection as SQLiteConnection).LastInsertRowId; 
        }
 
        public override string SqlDefine(ColumnMapping col)
        {
            string sql = "[" + col.Name + "] " + this.SqlMapping(col.Prop.PropertyType);
             
            if (col.Attribute.IsPrimaryKey)
            {
                sql += " PRIMARY KEY";
            }

            if (col.Attribute.IsAuto)
            {
                sql += " AUTOINCREMENT";
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
                return "TEXT";
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
                return "INTEGER";
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
                return "BOOLEAN";
            }
            else if (type == DataType.DateTime)
            {
                return "TIMESTAMP";
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
                return "INTEGER";
            }

            return "TEXT";
        }        
    }
}
