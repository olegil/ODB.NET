using System;  

namespace System.Data.ODB
{
    public abstract class EntityBase : IEntity
    {  
        public virtual string ObjectId { get; private set; }
        public virtual bool IsPersisted { get; protected set; }
         
        public EntityBase()
        {
            this.ObjectId = this.GetHashCode().ToString();            
        }      

        public IEntity Copy()
        {
            return (IEntity)this.MemberwiseClone();
        }
    }
}
