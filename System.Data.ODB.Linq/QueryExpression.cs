using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.ODB;

namespace System.Data.ODB.Linq
{
    public abstract class QueryExpression<T> : IExpression<T> where T : IEntity
    {
        public IQuery<T> Query { get; private set; }

        public QueryExpression(IQuery<T> query)  
        {
            this.Query = query;
        }

        public abstract void Translate(Expression expression);      

        public virtual Expression Visit(Expression exp)
        {
            if (exp == null)
                return exp;

            switch (exp.NodeType)
            {       
                case ExpressionType.Not:
                    return this.VisitUnary((UnaryExpression)exp);
              
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:            
                    return this.VisitBinary((BinaryExpression)exp);

                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);

                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);

                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp); 

                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }

            return e;
        }
                
        protected Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                this.Query.Select("*").From();             

                this.Visit(m.Arguments[0]);

                this.Query.Append(") AS T WHERE ");

                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                this.Visit(lambda.Body);

                return m;
            }

            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:

                    this.Query.NotEq(" NOT ");

                    this.Visit(u.Operand);

                    break;

                default:

                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }

            return u;
        }
        
        protected Expression VisitBinary(BinaryExpression b)
        {
            this.Query.Append(" (");

            this.Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.And:
                    this.Query.Append(" AND ");

                    break;

                case ExpressionType.Or:
                    this.Query.Append(" OR");

                    break;

                case ExpressionType.Equal:
                    this.Query.Append(" = ");

                    break;

                case ExpressionType.NotEqual:
                    this.Query.Append(" <> ");

                    break;

                case ExpressionType.LessThan:
                    this.Query.Append(" < ");

                    break;

                case ExpressionType.LessThanOrEqual:
                    this.Query.Append(" <= ");

                    break;

                case ExpressionType.GreaterThan:
                    this.Query.Append(" > ");

                    break;

                case ExpressionType.GreaterThanOrEqual:
                    this.Query.Append(" >= ");

                    break;

                default:

                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));

            }

            this.Visit(b.Right);

            this.Query.Append(") ");

            return b;
        }

        protected Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                this.Query.Append(m.Member.Name);

                return m;
            }

            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        protected virtual Expression VisitParameter(ParameterExpression p)
        {
            return p;
        }

        protected virtual Expression VisitLambda(LambdaExpression lambda)
        {
            Expression body = this.Visit(lambda.Body);

            if (body != lambda.Body)
            {
                return Expression.Lambda(lambda.Type, body, lambda.Parameters);
            }

            return lambda;
        } 
    }
}
