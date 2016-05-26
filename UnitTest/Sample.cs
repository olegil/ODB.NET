using System;
using System.Data.ODB;
using System.Data.ODB.Linq;
using System.Data.ODB.SQLite;
using System.Data.SQLite;

namespace UnitTest
{   
    public class User : OdbEntity
    {  
        public double Balance { get; set; }         
        public bool IsPermit { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }         
        public DateTime? Birthday { get; set; }
 
        public Address Shipping { get; set; }
    }
      
    public class Address : OdbEntity
    { 
        public string Flat { get; set; }        
        public string Street { get; set; } 
        public string City { get; set; }
    }    

    public class Order : OdbEntity
    {  
        public string PackageID { get; set; } 
        public string Remark { get; set; }                 
        public decimal Total { get; set; }
        public DateTime Date { get; set; }

        [Column(IsForeignKey = true)]
        public User User { get; set; }
    }

    public class OrderItem : OdbEntity
    {
        public int Quantity { get; set; }
        public DateTime CreateDate { get; set; }

        [Column(IsForeignKey = true)]
        public Order Order { get; set; }

        [Column(IsForeignKey = true)]
        public Product Item { get; set; }
    }
     
    public class Product : OdbEntity
    { 
        public string Name { get; set; }
        public double Price { get; set; }
        public string BID { get; set; }
    }

    public class Event : OdbEntity
    { 
        public int No { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Time { get; set; }
        public string Venue { get; set; }
    }

    public class ViewModel
    {
        public string Name { get; set; }
    }
 
    public class MyRepository : OdbContainer
    {
        public QueryTable<User> Users { get; set; }
        public QueryTable<OrderItem> OrderItems { get; set; }
        public QueryTable<Order> Orders { get; set; }
 
        public MyRepository()  
        {         
            this.Context = new SQLiteDataContext(new SQLiteConnection(string.Format(Command.SqliteconnStr, "test1.db3")));

            SQLiteProvider provider = new SQLiteProvider(this.Context);

            this.Users = provider.Create<User>();
            this.OrderItems = provider.Create<OrderItem>();
            this.Orders = provider.Create<Order>();                        
        }
    
        public bool Clear<T>() where T : IEntity
        {
            int n = this.Context.Query().Delete<T>().Execute();

            return n > 0;
        }

        public IQuery Collect<T>() where T : IEntity
        {
            return this.Context.Select<T>();
        }

        public IQuery Count<T>() where T : IEntity
        {
            return this.Context.Query().Count("Id").From<T>();
        }
    }
}
