using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB.MSSQL
{
    public class OdbSqlCommand : OdbCommand
    {
        public OdbSqlCommand(IDbContext db) : base(db)
        {
        }

        public override int ExecuteCreate<T>()
        {
            throw new NotImplementedException();
        }

        public virtual int Create(Type type)
        {
            string dbtype = "";

            List<string> cols = new List<string>();

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute colAttr = MappingHelper.GetColumnAttribute(pi);

                if (colAttr != null)
                {
                    if (!colAttr.IsForeignkey)
                    {
                        dbtype = this.TypeMapping(pi.PropertyType);
                    }
                    else
                    {
                        dbtype = this.TypeMapping(typeof(long));

                        this.Create(pi.PropertyType);
                    }

                    cols.Add(this.Define(pi.Name, dbtype, colAttr));
                }
            }

            cols.Add(string.Format("CONSTRAINT [PK_dbo.{0}] PRIMARY KEY (Id)", type.Name));

            return this.Create(type.Name, cols.ToArray());
        }

        public override int ExecuteInsertReturnId<T>(T t)
        {
            if (this.ExecuteInsert(t) > 0)
            {
                string table = MappingHelper.GetTableName(t.GetType());

                return this.Db.ExecuteScalar<int>(string.Format("SELECT Id FROM {0} WHERE Id = SCOPE_IDENTITY();", table), null);
            }

            return -1;
        }

        private string Define(string name, string dbtype, ColumnAttribute colAttr)
        {
            string col = "[" + name + "] " + dbtype;

            if (colAttr.Length > 0)
                col += "(" + colAttr.Length + ")";
            else
                col += "(MAX)";

            if (colAttr.IsAuto)
            {
                col += " IDENTITY(1,1)";
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

            return "TEXT";
        }
    }
}
