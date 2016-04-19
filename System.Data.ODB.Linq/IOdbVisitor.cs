using System.Linq.Expressions;

namespace System.Data.ODB
{
    public interface IOdbVisitor
    {
        IDbDataParameter[] GetParamters();
        string GetQueryText();
    }
}
