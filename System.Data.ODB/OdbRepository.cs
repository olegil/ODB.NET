using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.ODB
{
    public abstract class OdbRepository : IRepository, IDisposable
    {
        protected DbContext Db;        

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
