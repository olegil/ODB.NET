using System;
using System.Collections.Generic; 

namespace System.Data.ODB
{
    public interface IDbContainer
    {
        void Create<T>() where T : IEntity;

        void Remove<T>() where T : IEntity;

        int Store<T>(T t) where T : IEntity;

        int Delete<T>(T t) where T : IEntity;
                       
        IList<T> Get<T>() where T : IEntity;

        IQuery Collect<T>() where T : IEntity;      
    }
}
