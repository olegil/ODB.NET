using System;

namespace System.Data.ODB
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
                    Inherited = false, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {        
        public string Name { get; set; }
        public int Length { get; set; }       
        public bool IsAuto { get; set; }
        public bool IsNullable { get; set; }      
        public bool IsOmitted { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsModel { get; set; }

        public ColumnAttribute()
        {
            Name = "";
            Length = 0;

            IsAuto = false;               
            IsNullable = true;
            IsOmitted = false;
            IsPrimaryKey = false;
            IsForeignKey = false;
            IsModel = false;            
        }
    }
}
