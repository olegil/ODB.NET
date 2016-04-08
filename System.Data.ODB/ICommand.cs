using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public interface ICommand
    {
        int ExecuteCreate<T>() where T : IEntity;
        int ExecuteDrop<T>() where T : IEntity;
        int ExecuteInsert<T>(T t) where T : IEntity;
        int ExecuteInsertReturnId<T>(T t) where T : IEntity;
        int ExecuteUpdate<T>(T t) where T : IEntity;
        int ExecuteDelete<T>(T t) where T : IEntity;   
    }
}
