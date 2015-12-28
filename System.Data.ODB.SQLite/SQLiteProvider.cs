using System.Collections.Generic;
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

        public override object Execute(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
         
            SQLiteParser parser = new SQLiteParser();

            parser.Translate(expression);

            IDataReader sr = this.Db.ExecuteReader(parser.ToString(), parser.Parameters.ToArray());
 
            return Activator.CreateInstance(typeof(SQLiteReader<>).MakeGenericType(elementType), sr); 
        }                
    }
}
