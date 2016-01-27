﻿using System.Collections.Generic;
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
                this.Tables.Add(type.Name + " AS T" + index, "");

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute attr = MappingHelper.GetColumnAttribute(pi);

                if (attr != null)
                {
                    string col = "T" + index + "." + pi.Name;

                    this._cols.Add(col + " AS [" + col + "]");

                    if (attr.IsForeignkey && this.Level > 1)
                    {
                        this.Level--;

                        int next = this.Tables.Count;

                        this.Tables.Add(pi.PropertyType.Name + " AS T" + next, col  + " = T" + next + ".Id");

                        this.Visit(pi.PropertyType, next);

                        this.Level++;                         
                    }                    
                }
            }
        }
    }
}