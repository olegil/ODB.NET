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
        protected QueryProvider()
        {
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

        protected static class TypeSystem
        {
            public static Type GetElementType(Type type)
            {
                Type ienum = FindIEnumerable(type);
                if (ienum == null) return type;

                return ienum.GetGenericArguments()[0];
            }

            private static Type FindIEnumerable(Type type)
            {
                if (type == null || type == typeof(string))
                    return null;

                if (type.IsArray)
                    return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());

                if (type.IsGenericType)
                {
                    foreach (Type arg in type.GetGenericArguments())
                    {
                        Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                        if (ienum.IsAssignableFrom(type))
                        {
                            return ienum;
                        }
                    }
                }

                Type[] ifaces = type.GetInterfaces();

                if (ifaces != null && ifaces.Length > 0)
                {
                    foreach (Type iface in ifaces)
                    {
                        Type ienum = FindIEnumerable(iface);
                        if (ienum != null) return ienum;
                    }
                }

                if (type.BaseType != null && type.BaseType != typeof(object))
                {
                    return FindIEnumerable(type.BaseType);
                }

                return null;
            }
        }
    }
}
