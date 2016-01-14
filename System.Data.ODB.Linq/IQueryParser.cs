using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public interface IQueryParser
    {
        IDbDataParameter[] GetParamters();

        void Translate(Expression expression, int depth);
    }
}
