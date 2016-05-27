using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB
{
    public class OdbDiagram 
    {
        public int Depth { get; set; }
        public OdbTable Root { get; set; } 
        public Dictionary<string, OdbTable> Nodes { get; set; }

        private StringBuilder _sb;
        private List<string> _cols;

        private int level;

        public OdbDiagram(Type type, int depth)
        {
            this.Depth = depth; 

            this.Root = OdbMapping.CreateTable(type);
            this.Root.Diagram = this;

            this.Nodes = new Dictionary<string, OdbTable>(); 
        }
 
        public void Visit()
        {
            this.Nodes.Clear();

            this.Nodes.Add(this.Root.Name, this.Root);

            this.level = 1;

            this.visitTree(this.Root);
        }
        
        private void visitTree(OdbTable node)
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

                        childNode.Diagram = this;

                        this.Nodes.Add(childNode.Name, childNode);

                        this.visitTree(childNode);

                        this.level--;
                    }                    
                }                      
            }
        }
                      
        public OdbTable FindTable(string name)
        {
            if (this.Nodes.ContainsKey(name))
                return this.Nodes[name];

            return null;         
        }

        public OdbTable FindTable(Type type)
        {
            string name = OdbMapping.GetTableName(type);

            return this.FindTable(name);
        }

        public string GetChildNodes(OdbTable root)
        {
            this._sb = new StringBuilder();

            this.getNodes(root);

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

        public string[] GetColumns(OdbTable root)
        {
            this._cols = new List<string>();

            this.getColumns(root);

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
