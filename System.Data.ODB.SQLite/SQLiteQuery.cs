using System.Data.ODB;
using System.Data.SQLite;

namespace System.Data.ODB.SQLite
{
    public class SQLiteQuery<T> : QueryBuilder<T> 
        where T : IEntity
    { 
        public SQLiteQuery(DbContext db) : base(db)
        {            
        }

        public IQuery<T> Skip(int start)
        {
            this.SqlStr.Append(" LIMIT " + start.ToString());

            return this;
        }

        public IQuery<T> Take(int count)
        {
            this.SqlStr.Append(" , " + count.ToString());

            return this;
        }

        public override IDbDataParameter AddValue(object obj)
        {
            string name = "@p" + this.LastIndex();

            SQLiteParameter p = new SQLiteParameter(name, obj);

            p.DbType = MappingHelper.TypeConvert(obj);

            this.AddParameter(p);

            return p;
        }    
    }
}
