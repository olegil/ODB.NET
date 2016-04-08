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

        public override int ExecuteCreate<T>()
        {
            Type type = typeof(T);

            return this.Create(type);
        }

        public virtual int Create(Type type)
        { 
            string dbtype = "";

            List<string> cols = new List<string>();

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute colAttr = MappingHelper.GetColumnAttribute(pi);

                if (!colAttr.NotMapped)
                {
                    if (!colAttr.IsForeignkey)
                    {
                        dbtype = this.TypeMapping(pi.PropertyType);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(colAttr.Name))
                            colAttr.Name = pi.Name + "Id";

                        dbtype = this.TypeMapping(typeof(long));

                        this.Create(pi.PropertyType);
                    }

                    string colName = string.IsNullOrEmpty(colAttr.Name) ? pi.Name : colAttr.Name;

                    cols.Add(this.Define(colName, dbtype, colAttr));
                }
            }

            string table = MappingHelper.GetTableName(type);

            return this.Create(table, cols.ToArray());
        }

        public override int ExecuteInsertReturnId<T>(T t)
        {
            if (this.ExecuteInsert(t) > 0)
                return (int)(this.Db.Connection as SQLiteConnection).LastInsertRowId;

            return -1;
        }

        private string Define(string name, string dbtype, ColumnAttribute colAttr)
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

        private string TypeMapping(Type type)
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
