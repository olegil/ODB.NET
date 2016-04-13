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

        public override IDbDataParameter Bind(string name, object b, DbType dtype)
        {
            SQLiteParameter p = new SQLiteParameter(name, b);

            p.DbType = dtype;

            return p;
        } 
         
        public override IQuery<T> Skip(int start)
        {
            this._sql.Append(" LIMIT " + start.ToString());

            return this;
        }

        public override IQuery<T> Take(int count)
        {
            this._sql.Append(" , " + count.ToString());

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
    }
}
