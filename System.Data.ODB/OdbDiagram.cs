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

        private int _count;

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

            this._count = 0;
        }

        public virtual void Analyze(Type type)
        {
            this.Table.Add(new OdbTable() { Name = OdbMapping.GetTableName(type), Alias = "T" + this._count });
           
            foreach (OdbColumn col in OdbMapping.GetColumn(type))
            {
                ColumnAttribute attr = col.Attribute;
   
                string colName = Enclosed("T" + this._count) + "." + Enclosed(col.Name);

                if (!attr.IsForeignkey)
                {
                    this._cols.Add(colName + " AS " + Enclosed("T" + this._count + "." + col.Name));
                }
                else  
                {
                    if (this.Level > 1)
                    { 
                        this.Level--;

                        this._count++;

                        string joinKey = Enclosed("T" + this._count) + "." + Enclosed("Id");

                        this.ForigeKey.Add(colName, joinKey);

                        this.Analyze(col.GetColumnType());

                        this._count--;

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
