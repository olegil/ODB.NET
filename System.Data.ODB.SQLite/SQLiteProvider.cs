using System.Data.ODB.Linq;
using System.Linq.Expressions;

namespace System.Data.ODB.SQLite
{
    public class SQLiteProvider : QueryProvider
    { 
        public SQLiteProvider(IDbContext db) : base(db)
        {
        } 

        public QueryTable<T> Create<T>() where T : IEntity
        {
            return new QueryTable<T>(this);
        }

        public override object Execute(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);

            SQLiteVisitor visitor = new SQLiteVisitor(expression);
            visitor.Level = this.Db.Depth; 

            IDataReader sr = this.Db.ExecuteReader(visitor.GetQueryText(), visitor.GetParamters());

            return Activator.CreateInstance(typeof(EntityReader<>).MakeGenericType(elementType), new object[] { sr, this.Db.Depth });
        }

        public override string GetSQL(Expression expression)
        {
            SQLiteVisitor visitor = new SQLiteVisitor(expression);
            visitor.Level = this.Db.Depth;

            return visitor.GetQueryText();
        } 
    }
}
