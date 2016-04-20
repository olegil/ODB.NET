﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public class OdbTable
    {
        public string Name { get; set; }
        public string Alias { get; set; } 
        public List<string> Columns { get; set; }

        public OdbTable()
        {
            this.Columns = new List<string>();
        }            
    }
}
