using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public class OdbException : Exception
    {
        public OdbException(string message) : base(message)
        {             
        }         
    }
}
