using System.Text;
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

        public int Depth { get; private set; }
        private int n;

        public OdbDiagram(int depth)
        {
            this.Depth = depth;

            this.ForigeKey = new Dictionary<string, string>();

            this.Table = new List<OdbTable>();

            this._cols = new List<string>();

            this.n = 0;
        }

        public virtual void Analyze(Type type)
        {
            OdbTable table = new OdbTable() { Name = OdbMapping.GetTableName(type), Level = this.n };

            this.Table.Add(table);
           
            foreach (OdbColumn col in OdbMapping.GetColumn(type))
            {
                ColumnAttribute attr = col.Attribute;
   
                string colName = Enclosed(table.Alias) + "." + Enclosed(col.Name);

                if (!attr.IsForeignkey)
                {
                    this._cols.Add(colName + " AS " + Enclosed(table.Alias + "." + col.Name));
                }
                else  
                {
                    if (this.Depth > 1)
                    {
                        this.n++;

                        this.Depth--;
 
                        string joinKey = Enclosed("T" + n) + "." + Enclosed("Id");

                        this.ForigeKey.Add(colName, joinKey);

                        this.Analyze(col.GetColumnType());
 
                        this.Depth++;
                    }                     
                }                      
            }
        }

        public string GetAlias(string name)
        {
            OdbTable table = this.Table.Find(delegate (OdbTable t) { return t.Name == name; });

            if (table != null)
            {
                return table.Alias;
            }

            return "";
        }

        private static string Enclosed(string str)
        {
            return "[" + str + "]"; 
        }
    }
}
