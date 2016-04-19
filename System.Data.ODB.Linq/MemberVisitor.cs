using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
 
namespace System.Data.ODB.Linq
{
    public class MemberVisitor : OdbVisitor
    { 
        private string _alias;
        private string _name;

        private Expression _expression;
        private int level;

        public MemberVisitor(Expression expression)
        { 
            this._expression = expression;     
            this.level = 0;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            this.level++;
                
            if (m.Expression.NodeType == ExpressionType.MemberAccess)
            {
                if (this.level < 2)
                    this.Visit(m.Expression);

                this._name = m.Member.Name;
            }             
            else if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                string table = "";
           
                if (this.level > 1)
                {
                    table = OdbMapping.GetTableName(m.Type);                   
                }
                else 
                {
                    table = OdbMapping.GetTableName(m.Expression.Type);
                  
                    this._name = m.Member.Name;
                }

                this._alias = this.getAlias(table); 
            } 
            else
                throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));

            return m;
        } 

        private string getAlias(string name)
        {
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

            this.level = 0;

            this.Visit(this._expression);

            return Enclosed(this._alias) + "." + Enclosed(this._name);
        }
    }
}
