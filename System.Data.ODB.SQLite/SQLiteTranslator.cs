using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Data.SQLite;

namespace System.Data.ODB.Linq
{
    public class SQLiteTranslator : ExpressionVisitor
    { 
        public SQLiteTranslator()
        {
        }

        public void Parse(Expression expression)
        {
            //Type type = TypeSystem.GetElementType(expression.Type);

            this.Visit(expression); 
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression)e).Operand;
            }
            return e;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof(Queryable) && m.Method.Name == "Where")
            {             
                this.Visit(m.Arguments[0]);
                this.Add(" WHERE ");

                LambdaExpression lambda = (LambdaExpression)StripQuotes(m.Arguments[1]);

                this.Visit(lambda.Body);

                return m;
            }
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    this.Add(" NOT ");
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }
            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            this.Add("(");
            this.Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.And:
                    this.Add(" AND ");
                    break;
                case ExpressionType.Or:
                    this.Add(" OR");
                    break;
                case ExpressionType.Equal:
                    this.Add(" = ");
                    break;
                case ExpressionType.NotEqual:
                    this.Add(" <> ");
                    break;
                case ExpressionType.LessThan:
                    this.Add(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    this.Add(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    this.Add(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    this.Add(" >= ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }
            this.Visit(b.Right);
            this.Add(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            IQueryable q = c.Value as IQueryable;
            if (q != null)
            {
                // assume constant nodes w/ IQueryables are table references
                this.Add("SELECT * FROM ");
                this.Add(q.ElementType.Name);
            }
            else if (c.Value == null)
            {
                this.Add("NULL");
            }
            else {
                switch (Type.GetTypeCode(c.Value.GetType()))
                {
                    case TypeCode.Boolean:
                        this.Add(((bool)c.Value) ? "1" : "0");
                        break;
                    case TypeCode.String:                        
                    case TypeCode.Object:
                        string name = "@p" + Parameters.Count;

                        this.Add(name);
                        this.BindParam(name, c.Value);

                        break;
                    default:
                        this.Add(c.Value.ToString());

                        break;
                }
            }
            return c;
        }

        protected override Expression VisitMemberAccess(MemberExpression m)
        {
            if (m.Expression != null && m.Expression.NodeType == ExpressionType.Parameter)
            {
                this.Add(m.Member.Name);
                return m;
            }
            throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
        }

        public override IDbDataParameter BindParam(string name, object b)
        {
            SQLiteParameter p = new SQLiteParameter();

            p.ParameterName = name;
            p.Value = b;
            //p.Size = attr.Size;
            p.DbType = MappingHelper.TypeConvert(b);

            this.Parameters.Add(p);

            return p;
        }
    }
}
