using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public class OdbTree
    {
        private Dictionary<string, OdbTable> _list;

        private List<string> _cols;

        private StringBuilder sb;

        public OdbTree(Dictionary<string, OdbTable> list)
        {
            this._list = list;

            this.sb = new StringBuilder();
            this._cols = new List<string>();            
        }

        public string[] GetNodeColumns(OdbTable root)
        {            
            foreach (OdbColumn c in root.Columns)
            {
                string col = OdbDiagram.Enclosed(root.Alias) + "." + OdbDiagram.Enclosed(c.Name);

                this._cols.Add(col + " AS " + OdbDiagram.Enclosed(root.Alias + "." + c.Name));
            }

            foreach(OdbTable child in this._list.Values)
            {
                if (child.Parent == root.Id)
                {  
                    this.GetNodeColumns(child);
                }
            }

            return this._cols.ToArray();     
        }

        public string GetChildNodes(OdbTable root)
        {             
            foreach (OdbTable child in this._list.Values)
            {
                if (child.Parent == root.Id)
                {
                    string table = OdbDiagram.Enclosed(child.Name) + " AS " + child.Alias;

                    string key = OdbDiagram.Enclosed(child.Alias) + "." + OdbDiagram.Enclosed("Id");
                    string val = OdbDiagram.Enclosed(root.Alias) + "." + OdbDiagram.Enclosed(child.Foreignkey);

                    this.sb.Append(" LEFT JOIN " + table + " ON " + key + " = " + val);

                    this.GetChildNodes(child);
                }
            }

            return this.sb.ToString();
        }
    }
}
