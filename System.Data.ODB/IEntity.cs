using System;

namespace System.Data.ODB
{
    public interface IEntity
    {
        string EntityId { get; }
        bool IsPersisted { get; }
        IEntity Copy();    
    }
}
