using System.Collections.Generic;
using System.Reflection;

namespace System.Data.ODB
{
    public class TableVisitor 
    { 
        public Dictionary<string, string> Tables { get; private set; }

        private List<string> _cols;
       
        public string[] Colums
        {
            get
            {
                return this._cols.ToArray();
            }
        }

        public int Level { get; private set; }
        
        public TableVisitor(int level)
        {
            this.Level = level;

            this.Tables = new Dictionary<string, string>();
            this._cols = new List<string>();
        }

        public virtual void Visit(Type type, int index = 0)
        { 
            if (index == 0)
                this.Tables.Add(TableMapping(type, index), "");

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute attr = MappingHelper.GetColumnAttribute(pi);
 
                if (!attr.NotMapped)
                {
                    string colName = string.IsNullOrEmpty(attr.Name) ? pi.Name : attr.Name;

                    string col = Enclosed("T" + index) + "." + Enclosed(colName);

                    if (!attr.IsForeignkey)
                    {
                        this._cols.Add(col + " AS " + Enclosed("T" + index + "." + colName));
                    }
                    else  
                    {
                        if (this.Level > 1)
                        { 
                            this.Level--;

                            int next = index + 1;

                            string fkId = string.IsNullOrEmpty(attr.Name) ? "Id" : "";
 
                            string constraint = Enclosed("T" + index) + "." + Enclosed(colName + fkId);

                            string fkName = TableMapping(pi.PropertyType, next);
                              
                            string joinKey = Enclosed("T" + next) + "." + Enclosed("Id");

                            this.Tables.Add(fkName, constraint + " = " + joinKey);

                            this.Visit(pi.PropertyType, next);

                            this.Level++;
                        }                     
                    }                    
                }
            }
        }

        private static string TableMapping(Type type, int index)
        {
            string name = MappingHelper.GetTableName(type);
 
            return Enclosed(name) + " AS " + Enclosed("T" + index);              
        }

        private static string Enclosed(string str)
        {
            return "[" + str + "]"; 
        }
    }
}
