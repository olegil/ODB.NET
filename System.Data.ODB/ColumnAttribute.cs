using System;

namespace System.Data.ODB
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
                    Inherited = false, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {        
        public int Size { get; set; }
        public bool IsPrimaryKey { get; set;}
        public bool IsForeignkey { get; set; }
        public bool IsAuto { get; set; }
        public bool IsNullable { get; set; }      
              
        public ColumnAttribute()
        { 
        }
    }
}
