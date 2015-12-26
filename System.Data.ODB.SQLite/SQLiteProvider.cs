using System.Collections.Generic;
using System.Data.ODB.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.ODB.SQLite
{
    public class SQLiteProvider : QueryProvider
    {
        private SQLiteTranslator trans;

        public SQLiteProvider(IDbContext db) : base(db)
        {
            this.trans = new SQLiteTranslator();
        }     

        public override object Execute(Expression expression)
        {
            this.trans.Parse(expression);

            return this.Db.ExecuteReader(this.trans.SQL, this.trans.Parameters.ToArray());
        }                
    }
}
