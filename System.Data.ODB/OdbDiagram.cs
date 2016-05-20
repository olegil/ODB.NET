using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB
{
    public class OdbDiagram 
    {
        private OdbTable root; 
        public Dictionary<string, OdbTable> Nodes { get; set; }
       
        private int level;

        public OdbDiagram(OdbTable rootNode)
        {
            this.root = rootNode;

            this.Nodes = new Dictionary<string, OdbTable>();

            this.level = 1; 
        }
 
        public void Visit()
        {
            this.Nodes.Clear();

            this.Nodes.Add(this.root.Name, this.root);

            this.visitTree(this.root);
        }
        
        private void visitTree(OdbTable node)
        { 
            foreach (OdbColumn col in node.Columns)
            {
                if (col.Attribute.IsModel)
                { 
                    this.level++;

                    OdbTable childNode = OdbMapping.CreateTable(col.GetMapType());

                    childNode.Id = this.Nodes.Count;
                    childNode.Parent = node.Id;
                    childNode.Foreignkey = col.Name; 

                    this.Nodes.Add(childNode.Name, childNode);
 
                    this.visitTree(childNode);
 
                    this.level--;                                        
                }                      
            }
        }

        public OdbTree CreateTree()
        { 
            return new OdbTree(this.Nodes);
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

        public static string Enclosed(string str)
        {
            return "[" + str + "]"; 
        }
    }
}
