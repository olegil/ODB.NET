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

        public override T Execute<T>(Expression expression) 
        {
            return (T)Execute(expression);                       
        }

        public override object Execute(Expression expression)
        {
            Type type = TypeSystem.GetElementType(expression.Type);
  
            SQLiteVisitor visitor = new SQLiteVisitor(expression);
        
            IDataReader sr = this.Db.ExecuteReader(visitor.ToString(), visitor.GetParamters());

            if (DataType.OdbEntity.IsAssignableFrom(type))
            {
                return Activator.CreateInstance(typeof(OdbReader<>).MakeGenericType(type), new object[] { sr, visitor.Diagram });
            }

            return null;            
        }

        public override string GetSQL(Expression expression)
        { 
            SQLiteVisitor visitor = new SQLiteVisitor(expression); 

            return visitor.ToString();
        } 
    }
}
