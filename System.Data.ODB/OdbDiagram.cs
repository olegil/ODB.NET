using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB
{
    public class OdbDiagram 
    {
        public Dictionary<string, OdbTable> Nodes { get; set; }

        public int Depth { get; set; }
        public OdbTable Root { get; private set; } 
        
        private StringBuilder _sb;
        private List<string> _cols;

        private int level;

        public OdbDiagram(int depth)
        {
            this.Depth = depth;

            this.Nodes = new Dictionary<string, OdbTable>();
        }
 
        public void CreateTableList(Type type)
        {
            this.Root = OdbMapping.CreateTable(type);

            this.Nodes.Clear();

            this.Nodes.Add(this.Root.Name, this.Root);

            this.level = 1;

            this.findNodes(this.Root);
        }
        
        private void findNodes(OdbTable node)
        { 
            foreach (OdbColumn col in node.Columns)
            {
                if (col.Attribute.IsModel)
                {
                    if (this.level < this.Depth)
                    {
                        this.level++;

                        OdbTable childNode = OdbMapping.CreateTable(col.GetMapType());

                        childNode.Id = this.Nodes.Count;
                        childNode.Parent = node.Id;
                        childNode.Foreignkey = col.Name;

                        this.Nodes.Add(childNode.Name, childNode);

                        this.findNodes(childNode);

                        this.level--;
                    }                    
                }                      
            }
        }
                      
        public OdbTable GetTable(string name)
        {
            if (this.Nodes.ContainsKey(name))
                return this.Nodes[name];

            return null;         
        }

        public OdbTable GetTable(Type type)
        {
            string name = OdbMapping.GetTableName(type);

            return this.GetTable(name);
        }

        public string GetChildNodes(Type type)
        {
            OdbTable table = this.GetTable(type);

            return this.GetChildNodes(table);        
        }

        public string GetChildNodes(OdbTable table)
        {
            this._sb = new StringBuilder();

            this.getNodes(table);

            return this._sb.ToString();          
        }

        private void getNodes(OdbTable root)
        {
            foreach (OdbTable child in this.Nodes.Values)
            {
                if (child.Parent == root.Id)
                {
                    string table = Enclosed(child.Name) + " AS " + child.Alias;

                    string key = Enclosed(child.Alias) + "." + Enclosed("Id");
                    string val = Enclosed(root.Alias) + "." + Enclosed(child.Foreignkey);

                    this._sb.Append(" LEFT JOIN " + table + " ON " + key + " = " + val);

                    this.getNodes(child);
                }
            }
        }

        public string[] GetColumns(Type type)
        {
            OdbTable table = this.GetTable(type);

            return this.GetColumns(table);
        }

        public string[] GetColumns(OdbTable table)
        {
            this._cols = new List<string>();

            this.getColumns(table);

            return this._cols.ToArray();
        }

        private void getColumns(OdbTable root)
        {
            foreach (OdbColumn c in root.Columns)
            {
                string col = OdbDiagram.Enclosed(root.Alias) + "." + OdbDiagram.Enclosed(c.Name);

                this._cols.Add(col + " AS " + OdbDiagram.Enclosed(root.Alias + "." + c.Name));
            }

            foreach (OdbTable child in this.Nodes.Values)
            {
                if (child.Parent == root.Id)
                {
                    this.getColumns(child);
                }
            } 
        }

        public static string Enclosed(string str)
        {
            return "[" + str + "]"; 
        }
    }
}
