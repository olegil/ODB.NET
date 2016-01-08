using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace System.Data.ODB
{
    public class ColumnSelector 
    { 
        public List<string> Colums { get; private set; }
        public List<string> Tables { get; private set; }
        public List<string> Keys { get; private set; }

        public int Level { get; set; }
        private int _n = 0;

        private string col_format;

        public ColumnSelector()
        {
            this.Colums = new List<string>();
            this.Tables = new List<string>();
            this.Keys = new List<string>();

            col_format = "T{0}.{1} AS [T{2}.{3}]";
        }

        public void Find(Type type)
        {
            this.Tables.Add(type.Name + " AS T" + _n);

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute attr = MappingHelper.GetColumnAttribute(pi);

                if (attr != null)
                {
                    string colName = string.IsNullOrEmpty(attr.Name) ? pi.Name : attr.Name;

                    this.Colums.Add(string.Format(col_format, _n, colName, _n, colName));

                    if (attr.IsForeignkey && _n < this.Level - 1)
                    { 
                        this.Keys.Add("T" + _n + "." + colName + " = " + "T" + (_n+1) + ".Id");

                        _n++;

                        this.Find(pi.PropertyType);

                        _n--;
                    }                    
                }
            }
        }
    }
}
