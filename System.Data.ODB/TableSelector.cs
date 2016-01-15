using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace System.Data.ODB
{
    public class TableSelector 
    { 
        public List<string> Tables { get; private set; }

        private List<string> colums;
       
        public string Colums
        {
            get
            {
                return string.Join(",", this.colums.ToArray());
            }
        }

        public int Level { get; private set; }
        
        public TableSelector(int level)
        {
            this.Level = level;

            this.Tables = new List<string>();
            this.colums = new List<string>();
        }

        public virtual void Parser(Type type) 
        {             
            int index = 0;

            this.Tables.Add(type.Name + " AS T" + index);

            this.Find(type, index);
        }

        private void Find(Type type, int index)
        {
            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute attr = MappingHelper.GetColumnAttribute(pi);

                if (attr != null)
                {
                    string name = string.IsNullOrEmpty(attr.Name) ? pi.Name : attr.Name;
                    string col = "T" + index + "." + name;

                    this.colums.Add(col + " AS [" + col + "]");

                    if (attr.IsForeignkey && this.Level > 1)
                    {
                        this.Level--;

                        int next = this.Tables.Count;

                        this.Tables.Add(" LEFT JOIN " + pi.PropertyType.Name + " AS T" + next + " ON " + col  + " = T" + next + ".Id");

                        this.Find(pi.PropertyType, next);

                        this.Level++;                         
                    }                    
                }
            }
        }
    }
}
