using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public interface IOdbCommand
    {
        int Insert<T>(T t) where T : IEntity;
        int InsertReturnId<T>(T t) where T : IEntity;
        int Update<T>(T t) where T : IEntity;
        int Delete<T>(T t) where T : IEntity;
    }
}
