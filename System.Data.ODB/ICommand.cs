using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public interface ICommand
    {
        void ExecuteCreate<T>() where T : IEntity;
        void ExecuteDrop<T>() where T : IEntity;
        int ExecuteNonQuery<T>(T t) where T : IEntity;         
        int ExecuteDelete<T>(T t) where T : IEntity;

        IDbDataParameter CreateParameter();
    }
}
