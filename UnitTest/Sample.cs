using System;
using System.Data.ODB;
using System.Data.ODB.Linq;
using System.Data.ODB.SQLite;

namespace UnitTest
{   
    public class User : EntityBase
    {
        [Column(IsPrimaryKey = true, IsAuto = true, IsNullable = false)]
        public Int64 Id { get; private set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public DateTime Birthday { get; set; }

        [Column(IsForeignkey = true)]
        public Address Address { get; set; }
    }
      
    public class Address : EntityBase
    {
        [Column(IsPrimaryKey = true, IsAuto = true, IsNullable = false)]
        public Int64 Id { get; private set; }

        [Column]
        public string Flat { get; set; }

        [Column]
        public string Street { get; set; }

        [Column]
        public string City { get; set; }
    }

    public class Book : EntityBase
    {
        [Column(IsPrimaryKey = true, IsAuto = true, IsNullable = false)]
        public Int64 Id { get; private set; }

        public User User { get; set; }

        [Column]
        public string ISBN { get; set; }

        [Column]
        public DateTime Release { get; set; }        
    } 

    public class MyRepository : Repository
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
