using System.Linq.Expressions;

namespace System.Data.ODB.Linq
{
    public class MemberVisitor : OdbVisitor
    {   
        public MemberVisitor(Expression expression) : base(expression)
        {              
        }
  
        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.NodeType != ExpressionType.Constant)
            {
                string name = OdbMapping.GetTableName(m.Expression.Type);
             
                OdbTable table = this.Diagram.FindTable(name);

                this.SqlBuilder.Append(Enclosed(table.Alias) + "." + Enclosed(m.Member.Name));
            }
          
            return m;
        }
               
        public override string ToString()
        {
            if (this.Diagram == null)
                throw new OdbException("No Table."); 
 
            this.SqlBuilder.Length = 0;

            this.Visit(this._expression);
 
            return this.SqlBuilder.ToString();
        }
    }
}
