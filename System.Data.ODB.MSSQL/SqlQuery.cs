using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace System.Data.ODB.MSSQL
{
    public class SqlQuery<T> : OdbQuery<T>
        where T : IEntity
    {
        public SqlQuery(IDbContext db) : base(db)
        {
        }

        public override string AddParameter(int index, object b)
        {
            string name = "@p" + index;

            SqlParameter p = new SqlParameter(name, b);

            p.DbType = TypeHelper.Convert(b);
             
            this.DbParams.Add(p);

            return name;
        }

        public override T First()
        {
            throw new NotImplementedException();
        }

        public override IQuery<T> Skip(int start)
        {
            throw new NotImplementedException();
        }

        public override IQuery<T> Take(int count)
        {
            throw new NotImplementedException();
        }
    }
}
