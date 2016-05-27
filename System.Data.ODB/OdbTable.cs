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
        public string Name { get; set; }
        public int Parent { get; set; }
        public string Foreignkey { get; set; }
        public Type EntityType { get; set; }      
           
        public List<OdbColumn> Columns { get; set; }

        public OdbDiagram Diagram { get; set; }

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

        public string GetChilds()
        {
            return Diagram.GetChildNodes(this);
        }

        public string[] GetAllColums()
        {
            return Diagram.GetColumns(this);
        }
    }
}
