using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.ODB
{
    public abstract class Repository : IRepository, IDisposable
    {
        protected DbContext Db;        

        public Repository()
        { 
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Db.Dispose();
                }
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Begin()
        {
            this.Db.StartTrans();
        }

        public void SaveChanges()
        {
            if (this.Db.InTransaction)
            {
                this.Db.CommitTrans();
            }
        }

        public void Cancel()
        {
            this.Db.RollBack();
        } 

        public virtual int Add(IEntity t)
        {
            return this.Db.Insert(t);
        }

        public virtual int Update(IEntity t)
        {
            return this.Db.Update(t);
        }        

        public virtual int Delete(IEntity t)
        {
            return this.Db.Delete(t);
        }      
    }
} 
