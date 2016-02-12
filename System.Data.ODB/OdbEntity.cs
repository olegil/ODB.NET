﻿using System;  

namespace System.Data.ODB
{
    public abstract class OdbEntity : IEntity
    {
        //Primary Key
        [Column(IsPrimaryKey = true, IsAuto = true, IsNullable = false)]
        public long Id { get; set; }

        //Object state
        public bool IsPersisted { get; protected set; }       
    }
}