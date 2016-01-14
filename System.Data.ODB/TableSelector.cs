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
        public List<string> Colums { get; private set; }
       
        public int Level { get; private set; }
        private int _n = 0;

        public TableSelector(int level)
        {
            this.Level = level;

            this.Tables = new List<string>();
            this.Colums = new List<string>();
        }

        public virtual void Find(Type type)
        {
            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute attr = MappingHelper.GetColumnAttribute(pi);

                if (attr != null)
                {
                    string name = string.IsNullOrEmpty(attr.Name) ? pi.Name : attr.Name;
                    string col = "T" + _n + "." + name;

                    this.Colums.Add(col + " AS [" + col + "]");

                    if (attr.IsForeignkey && _n < this.Level - 1)
                    {  
                        _n++;

                        this.Tables.Add(" LEFT JOIN " + pi.PropertyType.Name + " AS T" + _n + " ON " + col  + " = T" + _n + ".Id");

                        this.Find(pi.PropertyType);

                        _n--;
                    }                    
                }
            }
        }
    }
}
