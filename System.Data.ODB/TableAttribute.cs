using System;
 
namespace System.Data.ODB
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property,
                    Inherited = false, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        public string Name { get; set; } 
        
        public TableAttribute(string name = "") 
        {
            this.Name = name;
        }    
    }
}
