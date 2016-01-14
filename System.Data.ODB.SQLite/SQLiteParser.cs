using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Data.SQLite;

namespace System.Data.ODB.Linq
{
    public class SQLiteParser : ExpressionVisitor, IQueryParser
    {
        private StringBuilder sb;
        private List<IDbDataParameter> ps;
        private bool _limit = false;     
        private int _length = 0;

        public int Depth { get; private set; }

        public SQLiteParser()
        {
            this.sb = new StringBuilder();        
            this.ps = new List<IDbDataParameter>(); 
        }

        public void Translate(Expression expression, int depth)
        {
            this.sb.Clear();
            this.ps.Clear();

            this.Depth = depth;
            this._length = 0;
            this._limit = false;

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
                if (!this._limit)
                {
                    this.sb.Append(" LIMIT ");

                    this._limit = true;
                }
                else
                {
                    this.sb.Append(" OFFSET ");
                }
                this.Visit(m.Arguments[1]);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Take")
            {
                this.Visit(m.Arguments[0]);
                if (!this._limit)
                {
                    this.sb.Append(" LIMIT ");

                    this._limit = true;
                }
                else
                {
                    this.sb.Append(", ");
                }
                this.Visit(m.Arguments[1]);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "OrderBy")
            {
                this.Visit(m.Arguments[0]);
                this.sb.Append(" ORDER BY ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]); 
                this.Visit(lambda.Body);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "OrderByDescending")
            {
                this.Visit(m.Arguments[0]);
                this.sb.Append(" ORDER BY ");                
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);                 
                this.Visit(lambda.Body);
                this.sb.Append(" DESC");
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "ThenBy")
            {
                this.Visit(m.Arguments[0]);
                this.sb.Append(", ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]); 
                this.Visit(lambda.Body);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "ThenByDescending")
            {
                this.Visit(m.Arguments[0]);
                this.sb.Append(", ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                this.sb.Append(" DESC");
            }          
            else if (m.Method.Name == "Contains")
            {
                this.Visit(m.Object);
                this.sb.Append(" LIKE ('%' || "); 
                this.Visit(m.Arguments[0]);
                this.sb.Append(" || '%')");
            }
            else if (m.Method.Name == "StartsWith")
            {
                this.Visit(m.Object);
                this.sb.Append(" LIKE (");                
                this.Visit(m.Arguments[0]);
                this.sb.Append(" || '%')");
            }
            else if (m.Method.Name == "EndsWith")
            {
                this.Visit(m.Object);
                this.sb.Append(" LIKE ('%' || ");
                this.Visit(m.Arguments[0]);
                this.sb.Append(")");
            }
            else if (m.Method.Name == "Equals")
            {
                this.Visit(m.Object);
                this.sb.Append(" = ");
                this.Visit(m.Arguments[0]);
            }
            else if (m.Method.Name == "Trim")
            {
                this.sb.Append(" TRIM(");
                this.Visit(m.Object);
                this.sb.Append(")");
            }
            else if (m.Method.Name == "ToLower")
            {
                this.sb.Append(" LOWER(");
                this.Visit(m.Object);
                this.sb.Append(")");
            }
            else if (m.Method.Name == "ToUpper")
            {
                this.sb.Append(" UPPER(");
                this.Visit(m.Object);
                this.sb.Append(")");
            }          
            else if (m.Method.Name == "IndexOf")
            {
                this.sb.Append(" INSTR(");
                this.Visit(m.Object);
                this.sb.Append(", ");
                this.Visit(m.Arguments[0]);
                this.sb.Append(")");
            }
            else if (m.Method.Name == "Substring")
            {
                this.sb.Append(" SUBSTR(");
                this.Visit(m.Object);
                this.sb.Append(", ");
                this.Visit(m.Arguments[0]);
                this.sb.Append(", ");
                this.Visit(m.Arguments[1]);
                this.sb.Append(")");
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
            this.Visit(b.Left);

            switch (b.NodeType)
            {               
                case ExpressionType.AndAlso:
                        this.sb.Append(" AND ");
                        break;
                case ExpressionType.OrElse:
                        this.sb.Append(" OR ");
                     break;
                case ExpressionType.Equal:
                        if (IsNullConstant(b.Right))
                            this.sb.Append(" IS ");
                        else
                            this.sb.Append(" = ");
                        break;
                case ExpressionType.NotEqual:
                        if (IsNullConstant(b.Right))
                            this.sb.Append(" IS NOT ");
                        else
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
 
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;

            if (q != null)
            {
                // assume constant nodes w/ IQueryables are table references
                TableSelector tsel = new TableSelector(this.Depth);
                tsel.Find(q.ElementType);

                this.sb.Append("SELECT ");
                this.sb.Append(string.Join(",", tsel.Colums.ToArray()));
                this.sb.Append(" FROM " + q.ElementType.Name + " AS T0");
            }
            else if (c.Value == null)
            {
                this.sb.Append("NULL");
            }
            else
            {
                string name = "@p" + this._length++;

                this.sb.Append(name);

                this.BindParam(name, c.Value);                               
            }

            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                this.sb.Append("T0." + m.Member.Name);
            }           
            else if (m.Member.DeclaringType == typeof(DateTime))
            {
                if (m.Member.Name == "Now")
                {
                    this.sb.Append("datetime()");
                }               
            }
            else if (m.Member.DeclaringType == typeof(string))
            {
                if (m.Member.Name == "Length")
                {
                    this.sb.Append("LENGTH(");

                    this.Visit(m.Expression);

                    this.sb.Append(")");
                }
            }           
            else if (m.Expression.NodeType == ExpressionType.Constant)
            {
                ConstantExpression mc = m.Expression as ConstantExpression;

                object b = (m.Member as FieldInfo).GetValue(mc.Value);
                           
                this.Visit(Expression.Constant(b));
            }
            else 
                throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
            
            return m;           
        }

        private IDbDataParameter BindParam(string name, object b)
        {
            SQLiteParameter p = new SQLiteParameter();

            p.ParameterName = name;
            p.Value = b;
            //p.Size = attr.Size;

            p.DbType = MappingHelper.TypeConvert(b);

            this.ps.Add(p);

            return p;
        } 

        public override string ToString()
        {
            return this.sb.ToString();
        }

        public IDbDataParameter[] GetParamters()
        {
            return this.ps.ToArray();
        }
    }
}
