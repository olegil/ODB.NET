using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.ODB
{
    interface IRepository
    {
        int Store(IEntity t);     
        int Delete(IEntity t);
    }
}
