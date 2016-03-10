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
  
        public override string Define(string name, string dbtype, ColumnAttribute colAttr)
        {
            string col = name + " " + dbtype;

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
 
        public override void AddParam(string name, object b, ColumnAttribute attr)
        {
            SQLiteParameter p = new SQLiteParameter(name, b);
        
            p.DbType = TypeHelper.Convert(b);

            this.DbParams.Add(p);         

            return;
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
