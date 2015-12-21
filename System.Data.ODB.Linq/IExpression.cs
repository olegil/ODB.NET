using System; 
using System.Linq.Expressions;
 
namespace System.Data.ODB.Linq
{
    public interface IExpression<T> where T : IEntity
    {
        Expression Visit(Expression expression);
    }
}
