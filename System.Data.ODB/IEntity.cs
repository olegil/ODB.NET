using System;

namespace System.Data.ODB
{
    public interface IEntity
    {
        int Id { get; set; }
        bool ModelState { get; set; }
    }
}
