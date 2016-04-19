using System.Linq;
using System.Linq.Expressions;

namespace System.Data.ODB.Linq
{
    public abstract class QueryProvider : IQueryProvider
    {
        protected IDbContext Db { get; set; } 

        protected QueryProvider(IDbContext db)
        {
            this.Db = db; 
        }

        IQueryable<T> IQueryProvider.CreateQuery<T>(Expression expression)
        {
            return new QueryTable<T>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);

            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(QueryTable<>).MakeGenericType(elementType), new object[] { this, expression });
            }

            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
  
        public abstract object Execute(Expression expression);

        public abstract T Execute<T>(Expression expression);                   

        public abstract string GetSQL(Expression expression); 
    }
}
