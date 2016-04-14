using System.Text;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Data.SQLite;

namespace System.Data.ODB.SQLite
{
    public class SQLiteQuery<T> : OdbQuery<T>
        where T : IEntity
    {      
        public SQLiteQuery()  
        { 
        }

        public override IDbDataParameter Bind(string name, object b, DbType dtype)
        {
            SQLiteParameter p = new SQLiteParameter(name, b);

            p.DbType = dtype;

            return p;
        } 
         
        public override IQuery<T> Skip(int start)
        {
            this._limit = start.ToString();
         
            return this;
        }

        public override IQuery<T> Take(int count)
        {
            if (!string.IsNullOrEmpty(this._limit))
            {
                this._limit += " , ";
            }

            this._limit += count.ToString();

            return this;
        }

        public override T First() 
        {
            this.Skip(0).Take(1);

            IList<T> list = this.Db.Get<T>(this);

            if (list.Count > 0)
                return list[0];

            return default(T);
        }
         
        public override string ToString()
        {
            string sql = this._sb.ToString();

            if (!string.IsNullOrEmpty(this._where))
            {
                sql += " WHERE " + this._where;
            }

            if (!string.IsNullOrEmpty(this._order))
            {
                sql += " ORDER BY " + this._order;
            }

            if (!string.IsNullOrEmpty(this._limit))
            {
                sql += " LIMIT " + this._limit;
            } 

            return sql;
        }
    }
}
