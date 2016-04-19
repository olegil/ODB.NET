using System.Linq.Expressions;

namespace System.Data.ODB
{
    public interface IVisitor
    {
        IDbDataParameter[] GetParamters();
        string GetQueryText();
    }
}
