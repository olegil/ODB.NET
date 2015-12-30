using System.Collections.Generic;
using System.Data.ODB;
using System.Data.ODB.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.ODB.SQLite
{
    public class SQLiteProvider : QueryProvider
    {
        public SQLiteProvider(IDbContext db) : base(db)
        {
        }     
 
        public override IQueryParser QueryParser()
        {
            return new SQLiteParser();
        }
    }
}
