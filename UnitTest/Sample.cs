using System;
using System.Data.ODB;
using System.Data.ODB.Linq;
using System.Data.ODB.SQLite;

namespace UnitTest
{   
    public class User : OdbEntity
    { 
        public float BID { get; set; }         
        public bool IsPermit { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }         
        public DateTime Birthday { get; set; }
    }
      
    public class Address : OdbEntity
    {  
        public string Flat { get; set; } 
        public string Street { get; set; } 
        public string City { get; set; }
    }
       
    public class Book : OdbEntity
    {        
        public string ISBN { get; set; }

        public DateTime Release { get; set; }        

        [Column(IsForeignkey = true, IsNullable = false)]
        public User User { get; set; }

        [Column(IsForeignkey = true, IsNullable = false)]
        public Publish Publish { get; set; }

        [Column(NotMapped = true)]
        public string Remark { get; set; }
    } 

    public class Publish : OdbEntity
    { 
        public string Name { get; set; }

        [Column(IsForeignkey = true, IsNullable = true)]
        public Address Address { get; set; }
    }

    public class EmsGroup : OdbEntity
    {
        [Column(IsForeignkey = true)]
        public User User { get; set; }

        [Column(IsForeignkey = true)]
        public Role Role { get; set; }
    }

    public class Role : OdbEntity
    {
        [Column]
        public string Name { get; set; }      
    }

    public class Order : OdbEntity
    {
        [Column(IsForeignkey = true)]
        public User User { get; set; }

        public string PackageID { get; set; }

        public DateTime Date { get; set; }
    }

    public class OrderItem : OdbEntity
    {
        [Column(IsForeignkey = true)]
        public Order Order { get; set; }
        
        public string Name { get; set; }
       
        public int Quantity { get; set; }

        [Column(NotMapped = true)]
        public decimal Price { get; set; }
    }
     
    public class MyRepository : OdbRepository
    {
        public QueryTable<User> Users { get; set; }

        public MyRepository()
        {           
            this.Db = new SQLiteContext(string.Format(Command.SqliteconnStr, Command.Dbname));

            QueryProvider provider = new SQLiteProvider(this.Db);

            this.Users = new QueryTable<User>(provider);

        }
    }
}
