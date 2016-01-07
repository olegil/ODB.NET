using System;  

namespace System.Data.ODB
{
    public abstract class EntityBase : IEntity
    {
        //Primary Key
        [Column(IsPrimaryKey = true, IsAuto = true, IsNullable = false)]
        public long Id { get; set; }

        //Object unique id
        public string ObjectId { get; private set; }

        //Object state
        public bool IsPersisted { get; protected set; }
         
        public EntityBase()
        {
            this.ObjectId = this.GetHashCode().ToString();            
        }    
    }
}
