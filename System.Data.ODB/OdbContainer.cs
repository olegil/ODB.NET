using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public class OdbContainer : IContainer, IDisposable
    {         
        private bool disposed = false;

        protected IContext Context;
        
        public OdbContainer()
        {
        }
               
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SetDepth(int n)
        {
            this.Context.Depth = n;
        }

        public void Create<T>() where T : IEntity
        {
            this.Context.ExecuteCreate<T>();
        }

        public void Remove<T>() where T : IEntity
        {
            this.Context.ExecuteDrop<T>();
        }

        public void Store<T>(T t) where T : IEntity
        {
            this.Context.ExecutePersist(t);
        }

        public void Delete<T>(T t) where T : IEntity
        {
            this.Context.ExecuteDelete(t);
        }
               
        public T Find<T>(int id) where T : IEntity
        {
            IQuery q = this.Context.Select<T>().Where("Id").Eq(id);

            return q.First<T>();
        } 
    }
}
