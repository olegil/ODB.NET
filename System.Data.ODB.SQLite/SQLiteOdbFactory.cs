using System;
using System.Data;
using System.Data.SQLite;

namespace System.Data.ODB.SQLite
{
    public class SQLiteOdbFactory
    {
        public static readonly SQLiteOdbFactory Instance =
            new SQLiteOdbFactory();

        public virtual IDbConnection CreateConnection(string str)
        {
            return new SQLiteConnection(str);
        }

        public virtual IDbDataParameter CreateParameter(string name, object b, DbType dtype)
        {
            SQLiteParameter p = new SQLiteParameter(name, b);

            p.DbType = dtype;

            return p;
        }
    }
}
