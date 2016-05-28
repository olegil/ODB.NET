using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.SQLite;
using System.Data.ODB.Linq;

namespace System.Data.ODB.SQLite
{
    public class SQLiteVisitor : OdbVisitor, IOdbVisitor
    {          
        private string _limit = "";   

        public SQLiteVisitor(Expression expression) : base(expression)
        {  
        }
         
        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Select")
            {
                this.Visit(m.Arguments[0]);
              
                this.SqlBuilder.Append(" ^ ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                if (OdbType.OdbEntity.IsAssignableFrom(lambda.Body.Type))
                {
                    this.SqlBuilder.Append(string.Join(",", this.GetColumns(lambda.Body.Type)));
                }

            }
            else if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {
                //visit From
                this.Visit(m.Arguments[0]);

                this.SqlBuilder.Append(" WHERE ");
                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);         
                      
                //visit Where
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
            else if (OdbType.OdbEntity.IsAssignableFrom(m.Type))
            {
                this.SqlBuilder.Append(string.Join(",", this.GetColumns(m.Type)));
            }
            else if (m.Expression.NodeType == ExpressionType.Constant)
            {
                this.Visit(m.Expression);
            }
            else
            {
                this.VisitMemberValue(m);
            }        

            return m;
        } 

        public override string Bind(int index, object b)
        {
            string name = "@p" + index;

            SQLiteParameter p = new SQLiteParameter();

            p.ParameterName = name;
            p.Value = b;
            //p.Size = attr.Size;
            p.DbType = OdbSqlType.Convert(b.GetType());

            this.Parameters.Add(p);

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
         
        public string GetQueryText()
        {
            this.SqlBuilder.Length = 0;
            this.Parameters.Clear();
            this._index = 0;
            this._limit = "";
 
            this.Visit(this._expression);
 
            return this.SqlBuilder.ToString(); 
        }

        public override string ToString()
        {
            string[] statm = this.GetQueryText().Split('^');

            string sql = "";

            if (statm.Length > 1)
            {
                sql = "SELECT " + statm[1] + statm[0];
            }
            else
            {
                Type type = TypeSystem.GetElementType(this._expression.Type);

                 sql = "SELECT " + this.GetColumns(type) + statm[0];                
            }

            return sql;        
        }
    } 
}
