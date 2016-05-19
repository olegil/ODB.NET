using System;
using System.Data.ODB;
using System.Data.ODB.Linq;
using System.Data.ODB.SQLite;
using System.Data.SQLite;

namespace UnitTest
{   
    public class User : IEntity
    { 
        public int Id { get; set; }
        public double Balance { get; set; }         
        public bool IsPermit { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }         
        public DateTime Birthday { get; set; } 
        public Address Shipping { get; set; }
    }
      
    public class Address : IEntity
    {
        public int Id { get; set; }
        public string Flat { get; set; }        
        public string Street { get; set; } 
        public string City { get; set; }
    }    

    public class Order : IEntity
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string PackageID { get; set; } 
        public string Remark { get; set; }                 
        public decimal Total { get; set; }
        public DateTime Date { get; set; }     
    }

    public class OrderItem : IEntity
    {
        public int Id { get; set; }
        public Order Order { get; set; }        
        public Product Item { get; set; }
        public int Quantity { get; set; }
        public DateTime CreateDate { get; set; }  
    }
     
    public class Product : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string BID { get; set; }
    }

    public class Event : IEntity
    {
        public int Id { get; set; }
        public int No { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Venue { get; set; }
    }

    public class MyRepository 
    {
        public QueryTable<User> Users { get; set; }
        public QueryTable<OrderItem> OrderItems { get; set; }
        public QueryTable<Order> Orders { get; set; }

        private IDbContext Db;

        public MyRepository()  
        {         
            this.Db = new SQLiteDataContext(new SQLiteConnection(string.Format(Command.SqliteconnStr, "test1.db3")));

            SQLiteProvider provider = new SQLiteProvider(this.Db);

            this.Users = provider.Create<User>();
            this.OrderItems = provider.Create<OrderItem>();
            this.Orders = provider.Create<Order>();
        }

        #region ORM        
        public virtual void Create<T>() where T : IEntity
        {
            this.Db.ExecuteCreate<T>();
        }

        public virtual void Remove<T>() where T : IEntity
        {
            this.Db.ExecuteDrop<T>();
        }

        public virtual void Store<T>(T t) where T : IEntity
        {
            this.Db.ExecutePersist(t);
        }

        public virtual int Delete<T>(T t) where T : IEntity
        {
            return this.Db.ExecuteDelete(t);
        }
        #endregion

        public bool Clear<T>() where T : IEntity
        {
            int n = this.Db.Query().Delete<T>().Execute();

            return n > 0;
        }

        public IQuery Collect<T>() where T : IEntity
        {
            return this.Db.Select<T>();
        }

        public IQuery Count<T>() where T : IEntity
        {
            return this.Db.Query().Count("Id").From<T>();
        }
    }
}
