using System.Collections.Generic;
using System.Data.SQLite;
using System.Data.ODB;

namespace System.Data.ODB.SQLite
{
    public class SQLiteQuery<T> : Query<T>
        where T : IEntity
    {
        public SQLiteQuery(IDbContext db) : base(db)
        {
        }

        public override IDbDataParameter BindParam(string name, object b, ColumnAttribute attr)
        {
            SQLiteParameter p = new SQLiteParameter();

            p.ParameterName = "@" + name;
            p.Value = b;
            //p.Size = attr.Size;
            p.DbType = MappingHelper.TypeConvert(b);

            this.Parameters.Add(p);

            return p;
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
    }
}
