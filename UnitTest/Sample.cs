﻿using System;
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
