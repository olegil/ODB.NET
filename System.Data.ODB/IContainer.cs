using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public interface IContainer 
    {
        void SetDepth(int n);

        void Create<T>() where T : IEntity;
        void Remove<T>() where T : IEntity;
        T Find<T>(int id) where T : IEntity;
        void Store<T>(T t) where T : IEntity; 
        void Delete<T>(T t) where T : IEntity;
    }
}
