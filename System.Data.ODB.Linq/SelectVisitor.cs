using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Data.ODB.Linq
{
    public class SelectVisitor : OdbVisitor
    {
        private Expression _expression;
        private StringBuilder _sb;

        public SelectVisitor(Expression expression)
        {
            this._expression = expression;

            this._sb = new StringBuilder();
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression.NodeType == ExpressionType.MemberAccess || m.Expression.NodeType == ExpressionType.Parameter)
            {
                MemberVisitor mv = new MemberVisitor(m);
                mv.Diagram = this.Diagram;

                this._sb.Append(mv.ToString());
            }

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

            this._sb.Append(string.Join(",", cols.ToArray()));

            return nex;
        }

        protected List<string> VisitMemberList(ReadOnlyCollection<Expression> original)
        {
            List<string> list = new List<string>();

            for (int i = 0, n = original.Count; i < n; i++)
            {
                MemberVisitor mv = new MemberVisitor(original[i]);
                mv.Diagram = this.Diagram;

                list.Add(mv.ToString());
            }

            return list;
        }

        public override string ToString()
        {
            this._sb.Length = 0;
           
            if (this.Diagram == null)
                throw new OdbException("No Table diagram."); 

            this.Visit(this._expression);

            return this._sb.ToString();
        }
    }
}
