using System;
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
                string name = OdbMapping.GetTableName(m.Expression.Type);
             
                string alias = this.Diagram.GetAlias(name);

                this.SqlBuilder.Append(Enclosed(alias) + "." + Enclosed(m.Member.Name));
            }

            return m;
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
