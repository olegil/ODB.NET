using System.Collections.Generic;
using System.Data.SQLite;

namespace System.Data.ODB.SQLite
{
    public class SQLiteQuery<T> : QueryBuilder<T> 
        where T : IEntity
    {
        private int length;
                
        public SQLiteQuery(DbContext db) : base(db)
        {
            this.length = 0;
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

            IList<T> list = this._db.Get(this);

            if (list.Count > 0)
                return list[0];

            return default(T);
        }

        public override IDbDataParameter AddValue(object obj)
        { 
            string name = "@p" + this.length++;

            SQLiteParameter p = new SQLiteParameter(name, obj);

            p.DbType = MappingHelper.TypeConvert(obj);

            this.AddParameter(p);

            return p;
        }    
    }
}
