using System;
using System.Collections.Generic; 

namespace System.Data.ODB
{
    public interface IDbContainer
    {
        void Create<T>() where T : IEntity;

        void Remove<T>() where T : IEntity;

        void Store<T>(T t) where T : IEntity;

        int Delete<T>(T t) where T : IEntity;
    }
}
