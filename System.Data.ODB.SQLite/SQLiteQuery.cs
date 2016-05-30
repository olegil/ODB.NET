using System.Collections.Generic;
using System.Data.SQLite;

namespace System.Data.ODB.SQLite
{
    public class SQLiteQuery : OdbQuery        
    {      
        public SQLiteQuery(IContext db) 
        {
            this.Db = db;
        }

        public override IDbDataParameter Bind(string name, object b, DbType dtype)
        {
            IDbDataParameter p = SQLiteOdbFactory.Instance.CreateParameter(name, b, dtype);
          
            return p;
        } 
         
        public override IQuery Skip(int start)
        {
            if (string.IsNullOrEmpty(this._limit))             
            {
                this.Take(-1);
            }

            this._limit += " OFFSET " + start;

            return this;
        }

        public override IQuery Take(int count)
        {               
            if (this._limit.IndexOf("OFFSET") > 0)
            {
                this._limit = this._limit.Replace("-1", count.ToString());
            }
            else
            {
                this._limit = count < 0 ? "-1" : count.ToString();
            }

            return this;
        }
 
        public override T First<T>() 
        {
            this.Skip(0).Take(1);

            IList<T> list = this.Db.ExecuteList<T>(this);

            if (list.Count > 0)
                return list[0];

            return default(T);
        }

        public override int Single()
        {
            return (int)this.Db.ExecuteScalar<long>(this.ToString(), this.Parameters.ToArray());
        }

        public override string ToString()
        {
            string sql = this._sb.ToString();

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

        public override int ExecuteReturnId()
        {
            this.Execute();

            return (int)(this.Db.Connection as SQLiteConnection).LastInsertRowId;
        }
    }
}
