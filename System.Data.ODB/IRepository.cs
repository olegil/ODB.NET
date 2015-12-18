using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.ODB
{
    interface IRepository
    {
        int Add(IEntity t);
        int Update(IEntity t);
        int Delete(IEntity t);
    }
}
