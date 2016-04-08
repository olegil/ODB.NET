using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public class OdbException : Exception
    {
        protected OdbException(string message) : base(message)
        {             
        }         
    }
}
