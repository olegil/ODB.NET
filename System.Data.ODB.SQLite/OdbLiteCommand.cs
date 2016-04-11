﻿using System.Collections.Generic;
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
 
        public override string Define(string name, string dbtype, ColumnAttribute colAttr)
        {
            string col = "[" + name + "] " + dbtype;
             
            if (colAttr.IsPrimaryKey)
            {
                col += " PRIMARY KEY";
            }

            if (colAttr.IsAuto)
            {
                col += " AUTOINCREMENT";
            }

            if (colAttr.IsNullable)
            {
                col += " NULL";
            }
            else
            {
                col += " NOT NULL";
            }

            return col;
        }

        public override string TypeMapping(Type type)
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

            return "TEXT";
        }        
    }
}