using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.ODB
{
    public abstract class OdbRepository : IRepository, IDisposable
    {
        protected OdbContext Db;        

        public OdbRepository()
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

        public virtual void Begin()
        {
            this.Db.StartTrans();
        }

        public virtual void SaveChanges()
        {
            if (this.Db.InTransaction)
            {
                this.Db.CommitTrans();
            }
        }

        public virtual void Cancel()
        {
            this.Db.RollBack();
        } 

        public virtual int Store(IEntity t)
        { 
            if (t != null)
                return this.Db.Insert(t);

            return -1; 
        } 

        public virtual int Delete(IEntity t)
        {
            return this.Db.Delete(t);
        }      
    }
} 
