using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace System.Data.ODB.MSSQL
{
    public class SqlQuery : OdbQuery          
    {
        private int _skip = 0;
        private int _take = 0;

        public SqlQuery(IDbContext db)  
        {
            this.Db = db;
        }

        public override IQuery Insert(string table, string[] cols)
        { 
            this._sb.Append("INSERT INTO ");
            this._sb.Append(Enclosed(table));

            this._sb.Append(" (");
            this._sb.Append(string.Join(", ", cols));
            this._sb.Append(")");

            this._sb.Append(" OUTPUT INSERTED.Id ");

            return this;
        }

        public override IDbDataParameter Bind(string name, object b, DbType dtype)
        {
            SqlParameter p = new SqlParameter(name, b);

            p.DbType = dtype;

            return p;
        }

        public override T First<T>()
        {
            this.Take(1);
            
            IList<T> list = this.Db.ExecuteList<T>(this);

            if (list.Count > 0)
                return list[0];

            return default(T);
        }

        public override int Single()
        {
            return this.Db.ExecuteScalar<int>(this.ToString(), this.Parameters.ToArray());
        }

        public override IQuery Skip(int start)
        { 
            if (string.IsNullOrEmpty(this._order))
            {
                this.OrderBy("Id").SortAsc();
            }

            this._skip = start;
             
            return this;
        }

        public override IQuery Take(int count)
        {
            this._take = count;
             
            return this;
        }

        public override string ToString()
        {
            string sql = this._sb.ToString();

            if (this._skip == 0 && !string.IsNullOrEmpty(this._order))
            {
                sql += " ORDER BY " + this._order;
            }

            if (this._skip > 0)
            {
                sql = "SELECT * FROM (" + sql.Insert(7, "ROW_NUMBER() OVER(ORDER BY " + this._order + ") AS [ROWNO],") + ") as P WHERE P.[ROWNO] > " + this._skip;
            }

            if (this._take > 0)
            {
                return sql.Insert(7, "TOP(" + this._take + ") ");
            }

            return sql; 
        }

        public override int ExecuteReturnId()
        { 
            return this.Db.ExecuteScalar<int>(this.ToString(), this.Parameters.ToArray());
        }
    }
}
