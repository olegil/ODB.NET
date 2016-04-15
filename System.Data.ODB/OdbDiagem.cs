using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB
{
    public class OdbDiagram 
    { 
        public Dictionary<string, string> ForigeKey { get; private set; }
        public List<OdbTable> Table { get; set; }

        private List<string> _cols;
       
        public string[] Colums
        {
            get
            {
                return this._cols.ToArray();
            }
        }

        public int Level { get; private set; }
        
        public OdbDiagram(int level)
        {
            this.Level = level;

            this.ForigeKey = new Dictionary<string, string>();
            this.Table = new List<OdbTable>();

            this._cols = new List<string>();
        }

        public virtual void Visit(Type type, int index = 0)
        {
            this.Table.Add(new OdbTable() { Name = OdbMapping.GetTableName(type), Alias = "T" + index });
           
            foreach (OdbColumn col in OdbMapping.GetColumn(type))
            {
                ColumnAttribute attr = col.Attribute;
   
                string colName = Enclosed("T" + index) + "." + Enclosed(col.Name);

                if (!attr.IsForeignkey)
                {
                    this._cols.Add(colName + " AS " + Enclosed("T" + index + "." + col.Name));
                }
                else  
                {
                    if (this.Level > 1)
                    { 
                        this.Level--;

                        int next = this.Table.Count; 

                        string joinKey = Enclosed("T" + next) + "." + Enclosed("Id");

                        this.ForigeKey.Add(colName, joinKey);

                        this.Visit(col.GetColumnType(), next);

                        this.Level++;
                    }                     
                }                      
            }
        }         

        private static string Enclosed(string str)
        {
            return "[" + str + "]"; 
        }
    }
}
