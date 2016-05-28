using System;
using System.Collections.Generic;

namespace System.Data.ODB
{
    public class OdbTable
    {
        public int Id { get; set; }  
        public string Name { get; set; }
        public int Parent { get; set; }
        public string Foreignkey { get; set; }
        public Type EntityType { get; set; }      
           
        public List<OdbColumn> Columns { get; set; }
    
        public string Alias
        {
            get
            {
                return "T" + this.Id;
            }
        }         

        public OdbTable(Type type)
        {
            this.Id = 0;
            this.Parent = -1;
            this.EntityType = type;         
            this.Name = OdbMapping.GetTableName(this.EntityType);

            this.Columns = new List<OdbColumn>();
        }     
    }
}
