using System;

namespace System.Data.ODB
{
    public interface IEntity
    {
        long Id { get; set; }       
        bool IsPersisted { get; }
    }
}
