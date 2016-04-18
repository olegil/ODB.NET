using System.Data.ODB.Linq;
using System.Linq.Expressions;

namespace System.Data.ODB.SQLite
{
    public class SQLiteProvider : QueryProvider
    {
        private SQLiteVisitor visitor;

        public SQLiteProvider(IDbContext db) : base(db)
        {
        }

        public override object Execute(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);

            this.visitor = new SQLiteVisitor(expression);

            this.visitor.Level = this.Db.Depth;

            string sql = this.visitor.GetQueryText();

            IDataReader sr = this.Db.ExecuteReader(sql, this.visitor.GetParamters());

            return Activator.CreateInstance(typeof(EntityReader<>).MakeGenericType(elementType), new object[] { sr, this.Db.Depth });
        }

        public override string GetSQL(Expression expression)
        {
            return this.visitor.ToString();
        } 
    }
}
