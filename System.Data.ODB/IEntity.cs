using System;

namespace System.Data.ODB
{
    public interface IEntity
    {
        long Id { get; }
        bool IsPersisted { get; }
    }
}
