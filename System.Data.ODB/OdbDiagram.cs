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

        public string[] Columns
        {
            get
            {
                return this._cols.ToArray();
            }
        }

        private int level;

        public OdbDiagram()
        {           
            this.ForigeKey = new Dictionary<string, string>();

            this.Table = new List<OdbTable>();

            this._cols = new List<string>();

            this.level = 1;
        }

        public virtual void Analyze(Type type)
        {
            OdbTable table = new OdbTable() { Name = OdbMapping.GetTableName(type), Level = this.Table.Count };

            this.Table.Add(table);
           
            foreach (OdbColumn col in OdbMapping.GetColumn(type))
            {
                string colName = Enclosed(table.Alias) + "." + Enclosed(col.Name);

                if (!col.IsForeignkey)
                {
                    this._cols.Add(colName + " AS " + Enclosed(table.Alias + "." + col.Name));
                }
                else  
                {
                    if (this.level < OdbConfig.Depth)
                    { 
                        this.level++;
 
                        string joinKey = Enclosed("T" + this.Table.Count) + "." + Enclosed("Id");

                        this.ForigeKey.Add(colName, joinKey);

                        this.Analyze(col.GetMapType());
 
                        this.level--;
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
