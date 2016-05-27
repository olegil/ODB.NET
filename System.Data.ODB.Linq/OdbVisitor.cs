using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Data.ODB.Linq
{
    public abstract class OdbVisitor : ExpressionVisitor
    {
        protected Expression _expression;
        public int Depth { get; set; }

        public OdbDiagram Diagram { get; set; }
        public StringBuilder SqlBuilder { get; set; }

        public List<IDbDataParameter> Parameters;
        protected int _index = 0;

        public OdbVisitor(Expression expr)
        {
            this._expression = expr;

            this.Parameters = new List<IDbDataParameter>();
            this.SqlBuilder = new StringBuilder();
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

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;

            if (q != null)
            { 
                this.Diagram = new OdbDiagram(q.ElementType, this.Depth);

                this.Diagram.Visit();

                this.SqlBuilder.Append(" FROM ");
                this.SqlBuilder.Append(Enclosed(this.Diagram.Root.Name));
                this.SqlBuilder.Append(" AS " + this.Diagram.Root.Alias);
                
                string joinText = this.Diagram.Root.GetChilds();

                this.SqlBuilder.Append(joinText);
            }
            else
            {
                this.AddParamter(c);
            }

            return c;
        }

        public void GetMemberValue(MemberExpression m)
        {
            if (m.Member is FieldInfo)
            {
                var fieldInfo = m.Member as FieldInfo;
                var constant = m.Expression as ConstantExpression;

                if (fieldInfo != null & constant != null)
                {
                    object value = fieldInfo.GetValue(constant.Value);

                    this.Visit(Expression.Constant(value));
                }
            }
            else if (m.Member is PropertyInfo)
            {
                MemberExpression mx = m;
                string name = m.Member.Name;

                while (mx.Expression is MemberExpression)
                {
                    mx = mx.Expression as MemberExpression;
                    name = mx.Member.Name + "." + name;
                }

                //check root expression type
                if (mx.Expression.NodeType == ExpressionType.Parameter)
                {
                    //dont get root
                    MemberVisitor mv = new MemberVisitor(m);
                    mv.Diagram = this.Diagram;

                    this.SqlBuilder.Append(mv.ToString());
                }
                else if (mx.Expression.NodeType == ExpressionType.Constant)
                {
                    var constant = (ConstantExpression)mx.Expression;

                    var obj = ((FieldInfo)mx.Member).GetValue(constant.Value);

                    //object value = ((PropertyInfo)m.Member).GetValue(fieldInfoValue, null);

                    var value = GetValue(obj, name);

                    this.Visit(Expression.Constant(value));
                }
            }
        }

        public static object GetValue(object obj, string propertyName)
        {
            var propertys = propertyName.Split('.');

            //skip first
            int i = 1;

            while (obj != null && i < propertys.Length)
            {
                var pi = obj.GetType().GetProperty(propertys[i++]);

                if (pi != null)
                    obj = pi.GetValue(obj);
                else
                    obj = null;
            }

            return obj;
        }

        public string GetColumns(Type type)
        { 
            OdbTable table = this.Diagram.FindTable(type);
 
            return string.Join(",", table.GetAllColums());
        }

        public void AddParamter(ConstantExpression c)
        {
            if (c.Value == null)
            {
                this.SqlBuilder.Append("NULL");
            }
            else
            {
                string name = this.Bind(this._index++, c.Value);

                this.SqlBuilder.Append(name);
            }
        }

        public virtual string Bind(int i, object b)
        {
            string name = "@p" + i; 

            return name;
        }

        public IDbDataParameter[] GetParamters()
        {
            return this.Parameters.ToArray();
        }
    }
}
