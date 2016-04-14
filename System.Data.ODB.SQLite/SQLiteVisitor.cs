using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Data.SQLite;
using System.Data.ODB.Linq;

namespace System.Data.ODB.SQLite
{ 

    public class SQLiteVisitor : ODBExpressionVisitor, IVisitor
    {
        private StringBuilder _sb;
        private List<IDbDataParameter> _parmas;
        private string _limit = "";       
        private int _index = 0;

        private string _alias = "T0";

        public int Depth { get; private set; }

        public SQLiteVisitor()
        {
            this._sb = new StringBuilder();
            this._parmas = new List<IDbDataParameter>();
        }

        public void Translate(Expression expression, int depth)
        {
            this._sb.Length = 0;
            this._parmas.Clear();

            this.Depth = depth;
            this._index = 0;
            
            this.Visit(expression);
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = (e as UnaryExpression).Operand;
            }

            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                this.Visit(m.Arguments[0]);
                this._sb.Append(" WHERE ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Select")
            {
                this.Visit(m.Arguments[0]);
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Skip")
            {
                this.Visit(m.Arguments[0]);

                this.setlimit();
                this._sb.Append(" OFFSET ");
                 
                this.Visit(m.Arguments[1]);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Take")
            {
                this.Visit(m.Arguments[0]);

                this.setlimit();
                this._sb.Append(", ");
                 
                this.Visit(m.Arguments[1]);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "OrderBy")
            {
                this.Visit(m.Arguments[0]);
                this._sb.Append(" ORDER BY ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "OrderByDescending")
            {
                this.Visit(m.Arguments[0]);
                this._sb.Append(" ORDER BY ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                this._sb.Append(" DESC");
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "ThenBy")
            {
                this.Visit(m.Arguments[0]);
                this._sb.Append(", ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "ThenByDescending")
            {
                this.Visit(m.Arguments[0]);
                this._sb.Append(", ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                this._sb.Append(" DESC");
            }
            else if (m.Method.Name == "Contains")
            {
                this.Visit(m.Object);
                this._sb.Append(" LIKE ('%' || ");
                this.Visit(m.Arguments[0]);
                this._sb.Append(" || '%')");
            }
            else if (m.Method.Name == "StartsWith")
            {
                this.Visit(m.Object);
                this._sb.Append(" LIKE (");
                this.Visit(m.Arguments[0]);
                this._sb.Append(" || '%')");
            }
            else if (m.Method.Name == "EndsWith")
            {
                this.Visit(m.Object);
                this._sb.Append(" LIKE ('%' || ");
                this.Visit(m.Arguments[0]);
                this._sb.Append(")");
            }
            else if (m.Method.Name == "Equals")
            {
                this.Visit(m.Object);
                this._sb.Append(" = ");
                this.Visit(m.Arguments[0]);
            }
            else if (m.Method.Name == "Trim")
            {
                this._sb.Append(" TRIM(");
                this.Visit(m.Object);
                this._sb.Append(")");
            }
            else if (m.Method.Name == "ToLower")
            {
                this._sb.Append(" LOWER(");
                this.Visit(m.Object);
                this._sb.Append(")");
            }
            else if (m.Method.Name == "ToUpper")
            {
                this._sb.Append(" UPPER(");
                this.Visit(m.Object);
                this._sb.Append(")");
            }
            else if (m.Method.Name == "IndexOf")
            {
                this._sb.Append(" INSTR(");
                this.Visit(m.Object);
                this._sb.Append(", ");
                this.Visit(m.Arguments[0]);
                this._sb.Append(")");
            }
            else if (m.Method.Name == "Substring")
            {
                this._sb.Append(" SUBSTR(");
                this.Visit(m.Object);
                this._sb.Append(", ");
                this.Visit(m.Arguments[0]);
                this._sb.Append(", ");
                this.Visit(m.Arguments[1]);
                this._sb.Append(")");
            }
            else
                throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));

            return m;
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    this._sb.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;

                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }

            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            this.Visit(b.Left);

            switch (b.NodeType)
            {
                case ExpressionType.AndAlso:
                    this._sb.Append(" AND ");
                    break;
                case ExpressionType.OrElse:
                    this._sb.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    if (IsNullConstant(b.Right))
                        this._sb.Append(" IS ");
                    else
                        this._sb.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    if (IsNullConstant(b.Right))
                        this._sb.Append(" IS NOT ");
                    else
                        this._sb.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    this._sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    this._sb.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    this._sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    this._sb.Append(" >= ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }

            this.Visit(b.Right);

            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;

            if (q != null)
            {
                TableVisitor tsel = new TableVisitor(this.Depth);
                tsel.Visit(q.ElementType);

                this._sb.Append("SELECT " + string.Join(",", tsel.Colums));
                this._sb.Append(" FROM " + MappingHelper.GetTableName(q.ElementType));
                this._sb.Append(" AS " + this._alias);
            }
            else if (c.Value == null)
            {
                this._sb.Append("NULL");
            }
            else
            {
                string name = this.Bind(this._index, c.Value);

                this._sb.Append(name);

                this._index++;
            }

            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                this._sb.Append(this.Enclosed(this._alias) + "." + this.Enclosed(m.Member.Name));
            }
            else if (m.Expression.NodeType == ExpressionType.Constant)
            {
                ConstantExpression mc = m.Expression as ConstantExpression;

                object b = (m.Member as FieldInfo).GetValue(mc.Value);

                this.Visit(Expression.Constant(b));
            }
            else if (m.Member.DeclaringType == typeof(DateTime))
            {
                if (m.Member.Name == "Now")
                {
                    this._sb.Append("datetime()");
                }
            }
            else if (m.Member.DeclaringType == typeof(string))
            {
                if (m.Member.Name == "Length")
                {
                    this._sb.Append("LENGTH(");

                    this.Visit(m.Expression);

                    this._sb.Append(")");
                }
            }
            else
                throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));

            return m;
        }

        private string Bind(int index, object b)
        {
            string name = "@p" + index;

            SQLiteParameter p = new SQLiteParameter();

            p.ParameterName = name;
            p.Value = b;
            //p.Size = attr.Size;

            p.DbType = SqlType.Convert(b.GetType());

            this._parmas.Add(p);

            return name;
        }

        private void setlimit()
        {
            if (this._limit == "")
            {
                this._limit = " LIMIT ";

                this._sb.Append(this._limit);
            }
        }

        private string Enclosed(string str)
        {
            return "[" + str + "]";
        }

        public override string ToString()
        {
            return this._sb.ToString();
        }

        public IDbDataParameter[] GetParamters()
        {
            return this._parmas.ToArray();
        }
    } 
}
