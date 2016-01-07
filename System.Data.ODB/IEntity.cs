using System;

namespace System.Data.ODB
{
    public interface IEntity
    {
        long Id { get; set; }
        string ObjectId { get; }
        bool IsPersisted { get; }
    }
}
