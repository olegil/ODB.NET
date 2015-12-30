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
        List<IDbDataParameter> Parameters { get; set; }

        void Translate(Expression expression);
    }
}
