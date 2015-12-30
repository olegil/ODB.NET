using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB.Linq
{
    public abstract class QueryProvider : IQueryProvider
    {
        protected IDbContext Db { get; set; } 

        protected QueryProvider(IDbContext db)
        {
            this.Db = db; 
        }

        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            return new QueryTable<S>(this, expression);
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

        S IQueryProvider.Execute<S>(Expression expression)
        {
            return (S)this.Execute(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return this.Execute(expression);
        } 

        public abstract object Execute(Expression expression);        
    }
}
