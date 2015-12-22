using System.Collections.Generic;
using System.Data.ODB.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.ODB.SQLite
{
    public class DbQueryProvider : QueryProvider
    {              
        public DbQueryProvider(IDbContext db) : base(db)
        {                     
        }     

        public override object Execute(Expression expression)
        {
            string sql = this.Translator.Translate(expression);

            Type type = TypeSystem.GetElementType(expression.Type);

            return new List<IEntity>();
        }                
    }
}
