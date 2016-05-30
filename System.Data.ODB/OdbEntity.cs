using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public abstract class OdbEntity : IEntity
    {
        public int Id { get; set; }

        [Column(IsOmitted = true)]
        public bool ModelState { get; set; }

        public OdbEntity()
        {
            this.Id = 0;
            this.ModelState = true;
        }
    }
}
