using System.Linq.Expressions;

namespace System.Data.ODB
{
    public interface IOdbVisitor
    {
        OdbDiagram Diagram { get; set; }
        IDbDataParameter[] GetParamters();
        string GetQueryText();
    }
}
