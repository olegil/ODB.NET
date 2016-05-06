using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Data.ODB
{
    public abstract class OdbContainer : IDbContainer  
    {            
        protected IDbContext DbConext { get; set; }

        public OdbContainer(IDbContext db)
        {
            this.DbConext = db;
        } 
         
        #region ORM        
        public virtual void Create<T>() where T : IEntity
        { 
            this.DbConext.ExecuteCreate<T>();
        }
        
        public virtual void Remove<T>() where T : IEntity
        {
            this.DbConext.ExecuteDrop<T>();
        }
 
        public virtual void Store<T>(T t) where T : IEntity
        { 
            this.DbConext.ExecutePersist(t);
        }
 
        public virtual int Delete<T>(T t) where T : IEntity
        { 
            return this.DbConext.ExecuteDelete(t); 
        }   
        #endregion
         
    }
}
