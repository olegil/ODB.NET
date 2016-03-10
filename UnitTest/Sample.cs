using System;
using System.Data.ODB;
using System.Data.ODB.Linq;
using System.Data.ODB.SQLite;

namespace UnitTest
{   
    public class User : OdbEntity
    {
        [Column]
        public string BID { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public DateTime Birthday { get; set; }
    }
      
    public class Address : OdbEntity
    { 
        [Column]
        public string Flat { get; set; }

        [Column]
        public string Street { get; set; }

        [Column]
        public string City { get; set; }
    }

    public class Book : OdbEntity
    {
        [Column]
        public string ISBN { get; set; }

        [Column]
        public DateTime Release { get; set; }        

        [Column(IsForeignkey = true, IsNullable = true)]
        public User User { get; set; }

        [Column(IsForeignkey = true, IsNullable = true)]
        public Publish Publish { get; set; }
    } 

    public class Publish : OdbEntity
    {
        [Column]
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

    public class OrderItem : OdbEntity
    {
        [Column]
        public int OrderId { get; set; }

        [Column]
        public long ItemId { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public decimal Price { get; set; }
    }

    public class RegUser : OdbEntity
    {
        [Column]
        public string gender { get; set; }

        [Column]
        public string title { get; set; }

        [Column]
        public string fname { get; set; }

        [Column]
        public string lname { get; set; }

        [Column]
        public string email { get; set; }

        [Column]
        public string company { get; set; }

        [Column]
        public string tel { get; set; }

        [Column]
        public string mobile { get; set; }

        [Column]
        public string relationship { get; set; }

        [Column]
        public string dietary { get; set; }

        [Column]
        public string others { get; set; }

        [Column]
        public string recommend { get; set; }

        [Column]
        public string newsletter { get; set; }

        [Column]
        public DateTime create_date { get; set; }

        [Column]
        public string privacy1 { get; set; }

        [Column]
        public string privacy2 { get; set; }

        [Column]
        public string privacy3 { get; set; }
    }

    public class MyRepository : OdbRepository
    {
        public QueryTable<User> Users { get; set; }

        public MyRepository()
        {           
            this.Db = new SQLiteContext(string.Format(Command.connectionString, Command.Dbname));

            QueryProvider provider = new SQLiteProvider(this.Db);

            this.Users = new QueryTable<User>(provider);

        }
    }
}
