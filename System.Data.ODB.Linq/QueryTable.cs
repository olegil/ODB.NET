using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB.Linq
{
    public class QueryTable<T> : IQueryable<T>, IQueryable, IOrderedQueryable<T>, IOrderedQueryable, IEnumerable<T>, IEnumerable
    {
        public QueryProvider Provider { get; private set; }
        public Expression Expression { get; private set; }

        public QueryTable(QueryProvider provider) 
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            this.Provider = provider;
            this.Expression = Expression.Constant(this);
        }

        public QueryTable(QueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            this.Provider = provider;
            this.Expression = expression;
        }      

        public Type ElementType
        {
            get { return typeof(T); }
        }

        IQueryProvider IQueryable.Provider
        {
            get
            {
                return this.Provider;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.Provider.Execute(Expression)).GetEnumerator();
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.Provider.Execute(Expression)).GetEnumerator();
        }

        public override string ToString()
        {
            return this.Provider.GetSQL(this.Expression);  
        }
    }
}
