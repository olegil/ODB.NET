using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.SQLite;

namespace System.Data.ODB.Linq
{
    public class SQLiteParser : ExpressionVisitor
    {
        private StringBuilder sb;

        public List<IDbDataParameter> Parameters
        {
            get; private set;
        }

        public SQLiteParser()
        {
            this.sb = new StringBuilder();

            this.Parameters = new List<IDbDataParameter>();
        }

        public void Translate(Expression expression)
        { 
            this.Visit(expression); 
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {             
                this.Visit(m.Arguments[0]);
                this.sb.Append(" WHERE ");

                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                this.Visit(lambda.Body);

                return m;
            }
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    this.sb.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            this.sb.Append("(");
            this.Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.And:
                    this.sb.Append(" AND ");
                    break;
                case ExpressionType.Or:
                    this.sb.Append(" OR");
                    break;
                case ExpressionType.Equal:
                    this.sb.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    this.sb.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    this.sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    this.sb.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    this.sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    this.sb.Append(" >= ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }
            this.Visit(b.Right);
            this.sb.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;

            if (q != null)
            {
                // assume constant nodes w/ IQueryables are table references
                this.sb.Append("SELECT * FROM ");
                this.sb.Append(q.ElementType.Name);
            }
            else if (c.Value == null)
            {
                this.sb.Append("NULL");
            }
            else
            {
                string name = "@p" + Parameters.Count;

                this.sb.Append(name);

                this.BindParam(name, c.Value);                               
            }

            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                this.sb.Append(m.Member.Name);
                return m;
            }
            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        public virtual IDbDataParameter BindParam(string name, object b)
        {
            SQLiteParameter p = new SQLiteParameter();

            p.ParameterName = name;
            p.Value = b;
            //p.Size = attr.Size;
            p.DbType = MappingHelper.TypeConvert(b);

            this.Parameters.Add(p);

            return p;
        }

        public override string ToString()
        {
            return this.sb.ToString();
        }
    }
}
