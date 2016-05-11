using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.SQLite;
using System.Data.ODB.Linq;

namespace System.Data.ODB.SQLite
{
    public class SQLiteVisitor : OdbVisitor, IOdbVisitor
    {  
        private List<IDbDataParameter> _parmas;
        private string _limit = "";       
        private int _index = 0;

        public SQLiteVisitor(Expression expression) : base()
        {
            this._expression = expression;                       
            this._parmas = new List<IDbDataParameter>(); 
        }
         
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Select")
            {
                this.Visit(m.Arguments[0]);

                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                SelectVisitor se = new SelectVisitor(lambda.Body);
                se.Diagram = this.Diagram;

                this.SqlBuilder.Insert(0, "SELECT ");
                this.SqlBuilder.Insert(7, se.ToString());                
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                this.Visit(m.Arguments[0]);
                this.SqlBuilder.Append(" WHERE ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);               
                this.Visit(lambda.Body);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Skip")
            {
                this.Visit(m.Arguments[0]);
                this.setlimit();
                this.SqlBuilder.Append(" OFFSET ");                 
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
                this.SqlBuilder.Append(" ORDER BY ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "OrderByDescending")
            {
                this.Visit(m.Arguments[0]);
                this.SqlBuilder.Append(" ORDER BY ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                this.SqlBuilder.Append(" DESC");
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "ThenBy")
            {
                this.Visit(m.Arguments[0]);
                this.SqlBuilder.Append(", ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "ThenByDescending")
            {
                this.Visit(m.Arguments[0]);
                this.SqlBuilder.Append(", ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);
                this.Visit(lambda.Body);
                this.SqlBuilder.Append(" DESC");
            } 
            else if (m.Method.Name == "Contains")
            {
                this.Visit(m.Object);
                this.SqlBuilder.Append(" LIKE ('%' || ");
                this.Visit(m.Arguments[0]);
                this.SqlBuilder.Append(" || '%')");
            }
            else if (m.Method.Name == "StartsWith")
            {
                this.Visit(m.Object);
                this.SqlBuilder.Append(" LIKE (");
                this.Visit(m.Arguments[0]);
                this.SqlBuilder.Append(" || '%')");
            }
            else if (m.Method.Name == "EndsWith")
            {
                this.Visit(m.Object);
                this.SqlBuilder.Append(" LIKE ('%' || ");
                this.Visit(m.Arguments[0]);
                this.SqlBuilder.Append(")");
            }
            else if (m.Method.Name == "Equals")
            {
                this.Visit(m.Object);
                this.SqlBuilder.Append(" = ");
                this.Visit(m.Arguments[0]);
            }
            else if (m.Method.Name == "Trim")
            {
                this.SqlBuilder.Append("TRIM(");
                this.Visit(m.Object);
                this.SqlBuilder.Append(")");
            }
            else if (m.Method.Name == "ToLower")
            {
                this.SqlBuilder.Append("LOWER(");
                this.Visit(m.Object);
                this.SqlBuilder.Append(")");
            }
            else if (m.Method.Name == "ToUpper")
            {
                this.SqlBuilder.Append("UPPER(");
                this.Visit(m.Object);
                this.SqlBuilder.Append(")");
            }
            else if (m.Method.Name == "IndexOf")
            {
                this.SqlBuilder.Append("INSTR(");
                this.Visit(m.Object);
                this.SqlBuilder.Append(", ");
                this.Visit(m.Arguments[0]);
                this.SqlBuilder.Append(")");
            }
            else if (m.Method.Name == "Substring")
            {                
                this.SqlBuilder.Append("SUBSTR(");
                this.Visit(m.Object);
                this.SqlBuilder.Append(", ");
                this.Visit(m.Arguments[0]);
                this.SqlBuilder.Append(", ");
                this.Visit(m.Arguments[1]);
                this.SqlBuilder.Append(")");
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
                    this.SqlBuilder.Append(" NOT ");
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
                    this.SqlBuilder.Append(" AND ");
                    break;
                case ExpressionType.OrElse:
                    this.SqlBuilder.Append(" OR ");
                    break;
                case ExpressionType.Equal:
                    if (IsNullConstant(b.Right))
                        this.SqlBuilder.Append(" IS ");
                    else
                        this.SqlBuilder.Append(" = ");
                    break;
                case ExpressionType.NotEqual:
                    if (IsNullConstant(b.Right))
                        this.SqlBuilder.Append(" IS NOT ");
                    else
                        this.SqlBuilder.Append(" <> ");
                    break;
                case ExpressionType.LessThan:
                    this.SqlBuilder.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    this.SqlBuilder.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    this.SqlBuilder.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    this.SqlBuilder.Append(" >= ");
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
                OdbTable table = OdbMapping.CreateTable(q.ElementType);

                this.Diagram = new OdbDiagram(table);

                this.Diagram.Visit();
 
                this.SqlBuilder.Append(" FROM ");
                this.SqlBuilder.Append(Enclosed(table.Name));
                this.SqlBuilder.Append(" AS " + table.Alias);

                OdbTree tree = this.Diagram.CreateTree();

                string joinText = tree.GetChildNodes(table);

                this.SqlBuilder.Append(joinText);
            }
            else if (c.Value == null)
            {
                this.SqlBuilder.Append("NULL");
            }
            else
            {
                string name = this.Bind(this._index++, c.Value);

                this.SqlBuilder.Append(name);
            }

            return c;
        } 

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Member.DeclaringType == typeof(DateTime) && m.Member.Name == "Now")
            {
                this.SqlBuilder.Append("datetime()");
            }
            else if (m.Member.DeclaringType == typeof(string) && m.Member.Name == "Length")
            {
                this.SqlBuilder.Append("LENGTH(");
                this.Visit(m.Expression);
                this.SqlBuilder.Append(")");
            }
            else
            {
                MemberVisitor mv = new MemberVisitor(m);
                mv.Diagram = this.Diagram;

                this.SqlBuilder.Append(mv.ToString());
            }

            return m;
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

                this.SqlBuilder.Append(this._limit);
            }
        }  

        public IDbDataParameter[] GetParamters()
        {
            return this._parmas.ToArray();
        }
         
        public string GetQueryText()
        {
            this.SqlBuilder.Length = 0;
            this._parmas.Clear();
            this._index = 0;
            this._limit = "";
 
            this.Visit(this._expression);
 
            return this.SqlBuilder.ToString(); 
        }

        public override string ToString()
        {
            string sql = this.GetQueryText();

            if (sql.IndexOf("SELECT") < 0)
            {
                Type type = TypeSystem.GetElementType(this._expression.Type);

                OdbTable table = this.Diagram.FindTable(type);

                OdbTree tree = this.Diagram.CreateTree();

                string cols = string.Join(",", tree.GetNodeColumns(table));
 
                sql = sql.Insert(0, "SELECT " + cols);
            }

            return sql;
        }
    } 
}
