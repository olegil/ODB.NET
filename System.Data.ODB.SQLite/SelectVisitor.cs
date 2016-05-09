using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.ODB.Linq;

namespace System.Data.ODB.SQLite
{
    public class SelectVisitor : OdbVisitor
    { 
        public SelectVisitor(Expression expression) : base()
        {
            this._expression = expression; 
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            Type type = m.Type;

            if (DataType.OdbEntity.IsAssignableFrom(type))
            {
                string cols = string.Join(",", this.Diagram.Colums.ToArray());

                this.SqlBuilder.Append(cols);
            }
            else
            {
                MemberVisitor mv = new MemberVisitor(m);
                mv.Diagram = this.Diagram;

                this.SqlBuilder.Append(mv.ToString());
            }
            
            return m;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.Name == "Trim")
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

        protected override NewExpression VisitNew(NewExpression nex)
        {
            List<string> cols = new List<string>();

            int n = 0;

            foreach (string c in this.VisitMemberList(nex.Arguments))
            {
                cols.Add(c + " AS " + nex.Members[n++].Name);
            }

            this.SqlBuilder.Append(string.Join(",", cols.ToArray()));

            return nex;
        }

        protected virtual IEnumerable<string> VisitMemberList(ReadOnlyCollection<Expression> original)
        { 
            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberVisitor mv = new MemberVisitor(original[i]);
                mv.Diagram = this.Diagram;

                yield return mv.ToString();
            }          
        }

        public override string ToString()
        {
            this.SqlBuilder.Length = 0;
           
            if (this.Diagram == null)
                throw new OdbException("No Table diagram."); 

            this.Visit(this._expression);

            return this.SqlBuilder.ToString();
        }
    }
}
