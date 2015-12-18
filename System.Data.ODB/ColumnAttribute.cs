using System;

namespace System.Data.ODB
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
                    Inherited = false, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        public string Name { get; set; }
        public int Size { get; set; }
        public bool IsPrimaryKey { get; set;}
        public bool IsForeignkey { get; set; }
        public bool IsAuto { get; set; }
        public bool IsNullable { get; set; }
        public bool EnumMapping { get; private set; }

        public ColumnAttribute(string name = "")
            : this(name, true)
        {             
        }

        public ColumnAttribute(string name, bool isNull)
            : this(name, true, false)
        {
        }
          
        public ColumnAttribute(string name, bool isNull, bool isEnum)
        {
            this.Name = name;
            this.IsNullable = isNull;   
            this.EnumMapping = isEnum;
        }
    }
}
