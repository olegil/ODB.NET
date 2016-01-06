using System;

namespace System.Data.ODB
{
    public interface IEntity
    {
        string ObjectId { get; }
        bool IsPersisted { get; }
        IEntity Copy();    
    }
}
