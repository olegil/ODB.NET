using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB
{
    public class OdbDiagram 
    {
        private OdbTable root; 
        public List<OdbTable> Nodes { get; set; }
       
        private int level;

        public OdbDiagram(OdbTable rootNode)
        {
            this.root = rootNode;
           
            this.Nodes = new List<OdbTable>();

            this.Nodes.Add(root);

            this.level = 1; 
        }
 
        public void Visit()
        {
            this.visitTree(this.root);
        }
        
        private void visitTree(OdbTable node)
        { 
            foreach (OdbColumn col in node.Columns)
            {
                if (col.IsForeignkey && this.level < OdbConfig.Depth)
                { 
                    this.level++;

                    OdbTable childNode = OdbMapping.CreateTable(col.GetMapType());

                    childNode.Id = this.Nodes.Count;
                    childNode.Parent = node.Id;

                    this.Nodes.Add(childNode);
 
                    this.visitTree(childNode);
 
                    this.level--;                                        
                }                      
            }
        }

        public OdbTree CreateTree()
        {
            OdbTree tree = new OdbTree(this.Nodes);

            return tree;
        }
        
        public OdbTable FindTable(string name)
        {
            OdbTable table = this.Nodes.Find(delegate (OdbTable t) { return t.Name == name; });

            return table;         
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
