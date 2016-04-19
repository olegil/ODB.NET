using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Data.SQLite;
using System.Data.ODB.Linq;

namespace System.Data.ODB.SQLite
{  
    public class SQLiteVisitor : OdbExpressionVisitor, IVisitor
    {
        private StringBuilder _sb;
        
        public OdbDiagram Diagram { get; set; }

        private Expression _expression;
        private List<IDbDataParameter> _parmas;
        private string _limit = "";       
        private int _index = 0;
         
        public int Level { get; set; }

        public SQLiteVisitor(Expression expression)
        {
            this._expression = expression;

            this._sb = new StringBuilder();                       
            this._parmas = new List<IDbDataParameter>();
        }

        private void init()
        {
            this._sb.Length = 0;
            this._parmas.Clear();                      
            this._index = 0;           
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
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Select")
            {
                this.Visit(m.Arguments[0]); 

                this._sb.Insert(0, "SELECT ");

                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                this.Visit(lambda.Body);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                this.Visit(m.Arguments[0]);

                this._sb.Append(" WHERE ");
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
                this.Diagram = new OdbDiagram(this.Level);
                this.Diagram.Analyze(q.ElementType);

                OdbTable table = this.Diagram.Table[0]; 

                this._sb.Append(" FROM ");
                this._sb.Append(Enclosed(table.Name));
                this._sb.Append(" AS " + table.Alias);

                int n = 1;

                foreach (KeyValuePair<string, string> tc in this.Diagram.ForigeKey)
                {
                    OdbTable tab = this.Diagram.Table[n++];

                    this._sb.Append(" LEFT JOIN " + Enclosed(tab.Name) + " AS " + tab.Alias + " ON " + tc.Key + " = " + tc.Value);
                }
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
            if (m.Expression.NodeType == ExpressionType.MemberAccess || m.Expression.NodeType == ExpressionType.Parameter)
            {
                MemberVisitor mv = new MemberVisitor(m);

                mv.Diagram = this.Diagram;

                this._sb.Append(mv.ToString());                 
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

        protected override NewExpression VisitNew(NewExpression nex)
        {
            IEnumerable<Expression> args = this.VisitExpressionList(nex.Arguments);

            List<string> cols = new List<string>();

            int n = 0;

            foreach (var s in args)
            { 
                MemberVisitor mv = new MemberVisitor(s);
                mv.Diagram = this.Diagram;

                cols.Add(mv.ToString() + " AS " + nex.Members[n++].Name);
            }

            this._sb.Insert(7, string.Join(",", cols.ToArray())); 

            if (args != nex.Arguments)
            {
                if (nex.Members != null)
                    return Expression.New(nex.Constructor, args, nex.Members);
                else
                    return Expression.New(nex.Constructor, args);
            }

            return nex;
        }

        protected override ReadOnlyCollection<Expression> VisitExpressionList(ReadOnlyCollection<Expression> original)
        {
            List<Expression> list = null;

            for (int i = 0, n = original.Count; i < n; i++)
            {
                Expression p =  original[i] as MemberExpression;
                if (list != null)
                {
                    list.Add(p);
                }
                else if (p != original[i])
                {
                    list = new List<Expression>(n);
                    for (int j = 0; j < i; j++)
                    {
                        list.Add(original[j]);
                    }
                    list.Add(p);
                }
            }

            if (list != null)
            {
                return list.AsReadOnly();
            }

            return original;
        }

        private string Bind(int index, object b)
        {
            string name = "@p" + index;

            SQLiteParameter p = new SQLiteParameter();

            p.ParameterName = name;
            p.Value = b;
            //p.Size = attr.Size;

            p.DbType = OdbSqlType.Convert(b.GetType());

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

        public IDbDataParameter[] GetParamters()
        {
            return this._parmas.ToArray();
        }
         
        public string GetQueryText()
        {
            this.init();

            this.Visit(this._expression);

            string sql = this._sb.ToString();

            if (sql.IndexOf("SELECT") < 0)
            {
                string select = "SELECT " + string.Join(",", this.Diagram.Colums);

                sql = sql.Insert(0, select);
            }

            return sql;
        }
    } 
}
