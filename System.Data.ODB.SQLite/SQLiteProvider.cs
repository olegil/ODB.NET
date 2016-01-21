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

        public override object Execute(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);

            IVisitor expr = new SQLiteVisitor();

            expr.Translate(expression, this.Db.Depth);

            IDataReader sr = this.Db.ExecuteReader(expr.ToString(), expr.GetParamters());

            return Activator.CreateInstance(typeof(EntityReader<>).MakeGenericType(elementType), new object[] { sr, this.Db.Depth });
        }

        public override string GetSQL(Expression expression)
        {
            IVisitor expr = new SQLiteVisitor();

            expr.Translate(expression, this.Db.Depth);

            return expr.ToString();
        } 
    }
}
