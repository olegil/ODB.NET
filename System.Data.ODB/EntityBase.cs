using System;  

namespace System.Data.ODB
{
    public abstract class EntityBase : IEntity
    {  
        public virtual string EntityId { get; private set; }
        public virtual bool IsPersisted { get; protected set; }

        public EntityBase()
        {
            this.EntityId = this.GetHashCode().ToString();            
        }      

        public IEntity Copy()
        {
            return (IEntity)this.MemberwiseClone();
        }
    }
}
