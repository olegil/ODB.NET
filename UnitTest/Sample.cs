using System;
using System.Data.ODB;

namespace UnitTest
{
    public class User : EntityBase
    {
        [Column(IsPrimaryKey = true, IsAuto = true, IsNullable = false)]
        public Int64 Id { get; private set; }

        [Column]
        public string Name { get; set; }        
    }
    
    public class Book : EntityBase
    {
        [Column(IsPrimaryKey = true, IsAuto = true, IsNullable = false)]
        public Int64 Id { get; private set; }

        [Column]
        public Int64 UserId { get; set; }
       
        [Column]
        public DateTime Release { get; set; }        
    }
 
    public class Address : EntityBase
    {
        [Column(IsPrimaryKey = true, IsAuto = true, IsNullable = false)]
        public Int64 Id { get; private set; }

        [Column]
        public int Flat { get; set; }

        [Column]
        public string Street { get; set; }
    }
}
