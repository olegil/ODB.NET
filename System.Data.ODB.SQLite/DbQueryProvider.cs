using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.ODB;
using System.Data.ODB.Linq;
using System.Linq.Expressions;

namespace System.Data.ODB.SQLite
{
    public class DbQueryProvider : QueryProvider
    { 
        private IDbCcontext _db;
       
        public DbQueryProvider(DbContext db)
        {
            this._db = db;           
        }

        public override object Execute(Expression expression)
        {
            IQuery query = this._db.BuildSQL();

            SQLiteExpression parser = new SQLiteExpression(query);

            Type type = TypeSystem.GetElementType(expression.Type);

            parser.Translate(expression);
            //
            return null;
        }                
    }
}
