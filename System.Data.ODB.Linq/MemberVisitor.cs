using System.Linq;
using System.Linq.Expressions;

namespace System.Data.ODB.Linq
{
    public class MemberVisitor : OdbVisitor
    {   
        public MemberVisitor(Expression expression) : base()
        { 
            this._expression = expression;      
        }
  
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null)
            {
                string alias = this.getAlias(m.Expression.Type);

                this.SqlBuilder.Append(Enclosed(alias) + "." + Enclosed(m.Member.Name));
            }

            return m;
        }

        private string getAlias(Type type)
        {
            string name = OdbMapping.GetTableName(type);

            OdbTable table = this.Diagram.Table.Where(t => t.Name == name).FirstOrDefault();

            if (table != null)
            {
                return table.Alias;
            }

            return "";
        }

        public override string ToString()
        {
            if (this.Diagram == null)
                throw new OdbException("No Table diagram."); 
 
            this.SqlBuilder.Length = 0;

            this.Visit(this._expression);
 
            return this.SqlBuilder.ToString();
        }
    }
}
