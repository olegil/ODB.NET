using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public class OdbTable
    {
        public int Id { get; set; }                 
        public string Name { get; private set; }
        public int Parent { get; set; }
        public string Foreignkey { get; set; }
        public Type EntiType { get; set; }      
           
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

            this.EntiType = type;
            this.Name = OdbMapping.GetTableName(type);

            this.Columns = new List<OdbColumn>();
        }                  
    }
}
