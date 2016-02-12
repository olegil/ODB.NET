using System.Text;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Data.SQLite;

namespace System.Data.ODB.SQLite
{
    public class SQLiteQuery<T> : OdbQuery<T>
        where T : IEntity
    {
        public SQLiteQuery(IDbContext db) : base(db)
        {
        }
  
        public override string AddColumn(string name, string dbtype, ColumnAttribute colAttr)
        {
            string def = name + " " + dbtype;

            if (colAttr.IsPrimaryKey)
            {
                def += " PRIMARY KEY";
            }

            if (colAttr.IsAuto)
            {
                def += " AUTOINCREMENT";
            }

            if (colAttr.IsNullable)
            {
                def += " NULL";
            }
            else
            {
                def += " NOT NULL";
            }

            return def;
        }
 
        public override IDbDataParameter BindParam(string name, object b, ColumnAttribute attr)
        {
            SQLiteParameter p = new SQLiteParameter();

            p.ParameterName = "@" + name;
            p.Value = b;
            //p.Size = attr.Size;
            p.DbType = TypeHelper.Convert(b);          

            return p;
        }

        public override IQuery<T> Skip(int start)
        {
            this._sb.Append(" LIMIT " + start.ToString());

            return this;
        }

        public override IQuery<T> Take(int count)
        {
            this._sb.Append(" , " + count.ToString());

            return this;
        }

        public override T First() 
        {
            this.Skip(0).Take(1);

            IList<T> list = this._db.Get<T>(this);

            if (list.Count > 0)
                return list[0];

            return default(T);
        } 

        public override string TypeMapping(Type type)
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
