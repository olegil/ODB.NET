using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Data.ODB
{
    public abstract class OdbQuery : IQuery        
    {
        protected StringBuilder _sb;

        protected IContext Db;

        public List<IDbDataParameter> Parameters { get; set; }

        private string _alias;

        protected string _limit;
        protected string _order;

        public OdbDiagram Diagram { get; set; }
         
        public OdbQuery( )
        { 
            this._sb = new StringBuilder();
             
            this.Parameters = new List<IDbDataParameter>();
  
            this._limit = "";
            this._order = "";
        }             

        public virtual IQuery Select(string[] cols)
        {
            this._sb.Append("SELECT ");
            this._sb.Append(string.Join(",", cols));

            return this;
        }    

        public virtual IQuery From<T>() where T : IEntity 
        {
            string table = OdbMapping.GetTableName(typeof(T));

            return this.From(table);
        }

        public virtual IQuery From(string table, string alias = "")
        {
            this._sb.Append(" FROM ");

            this._sb.Append(Enclosed(table));

            if (!string.IsNullOrEmpty(alias))
            {
                this._alias = alias;

                return this.As(alias);
            }

            return this;
        }

        public virtual IQuery Insert(string table, string[] cols) 
        {
            this._sb.Append("INSERT INTO "); 
            this._sb.Append(Enclosed(table));

            this._sb.Append(" (");    
            this._sb.Append(string.Join(", ", cols));
            this._sb.Append(")");                

            return this;
        }

        public virtual IQuery Values(string[] cols)
        {
            this._sb.Append(" VALUES (");
            this._sb.Append(string.Join(", ", cols));
            this._sb.Append(")");

            return this;
        }

        public virtual IQuery Update(string table)
        {
            this._sb.Append("UPDATE "); 

            this._sb.Append(Enclosed(table));

            return this;
        }

        public virtual IQuery Set(string[] cols)
        {
            this._sb.Append(" SET ");
            this._sb.Append(string.Join(", ", cols));

            return this;
        }

        public virtual IQuery Delete<T>() where T : IEntity
        {
            this._sb.Append("DELETE");

            return this.From<T>();
        }
          
        public virtual IQuery Where(string str)
        {
            this._sb.Append(" WHERE ");
            this._sb.Append(this.AddAlias(str));

            return this;
        }
                
        public virtual IQuery And(string str)
        {
            this._sb.Append(" AND ");
            this._sb.Append(this.AddAlias(str)); 

            return this;
        }

        public virtual IQuery Or(string str)
        {
            this._sb.Append(" OR ");
            this._sb.Append(this.AddAlias(str));  

            return this;
        }

        public virtual IQuery Equal(string str)
        {
            this._sb.Append(" = ");
            this._sb.Append(str);

            return this;
        } 

        public virtual IQuery Eq(object val)
        {
            this._sb.Append(" = ");
            this._sb.Append(this.Add(val));

            return this;
        }

        public virtual IQuery NotEq(object val)
        {
            this._sb.Append(" <> ");
            this._sb.Append(this.Add(val));

            return this;
        }

        public virtual IQuery Gt(object val)
        {
            this._sb.Append(" > ");
            this._sb.Append(this.Add(val));

            return this;
        }

        public virtual IQuery Lt(object val)
        {
            this._sb.Append(" < ");
            this._sb.Append(this.Add(val));

            return this;
        }

        public virtual IQuery Gte(object val)
        {
            this._sb.Append(" >= ");
            this._sb.Append(this.Add(val));
         
            return this;
        }

        public virtual IQuery Lte(object val)
        {
            this._sb.Append(" <= ");
            this._sb.Append(this.Add(val));
             
            return this;
        } 

        public virtual IQuery Like(string str)
        {
            this._sb.Append(" LIKE ");
            this._sb.Append(this.Add("%" + str + "%"));
             
            return this;
        }

        public virtual IQuery OrderBy(string str)
        {
            this._order = this.AddAlias(str);
           
            return this;
        }

        public virtual IQuery SortAsc()
        {
            this._order += " ASC";

            return this;
        }

        public virtual IQuery SortDesc()
        {
            this._order += " DESC";
      
            return this;
        }

        public virtual IQuery Join<T1>() where T1 : IEntity
        {
            Type type = typeof(T1);

            return this.Join(OdbMapping.GetTableName(type));
        }

        public virtual IQuery Join(string table)
        {
            this._sb.Append(" JOIN ");
            this._sb.Append(Enclosed(table));

            return this;
        }

        public virtual IQuery LeftJoin<T1>() where T1 : IEntity
        {
            Type type = typeof(T1);

            return this.LeftJoin(OdbMapping.GetTableName(type));
        }

        public virtual IQuery LeftJoin(string table)
        {
            this._sb.Append(" LEFT");

            return this.Join(table);
        }

        public virtual IQuery As(string str)
        {
            this._sb.Append(" AS ");
            this._sb.Append(Enclosed(str));

            return this;
        }

        public virtual IQuery On(string str)
        {
            this._sb.Append(" ON ");
            this._sb.Append(str);

            return this;
        }

        public IQuery Not()
        {
            this._sb.Append(" NOT ");

            return this;
        }

        public virtual IQuery Count(string str)
        {
            string[] cols = { " COUNT(" + str + ")" };

            return this.Select(cols); 
        }

        public abstract IQuery Skip(int start);

        public abstract IQuery Take(int count);
         
        public virtual string Add(object b)
        {
            string name = "@p" + this.Parameters.Count;

            IDbDataParameter param = this.Bind(name, b, OdbSqlType.Convert(b.GetType()));

            this.Parameters.Add(param);
  
            return name;
        }

        public abstract IDbDataParameter Bind(string name, object b, DbType dtype);        

        public virtual string AddAlias(string str)
        {
            if (!string.IsNullOrEmpty(this._alias))
            {
                return Enclosed(this._alias) + "." + Enclosed(str);
            }
 
            return Enclosed(str);
        } 

        public IQuery Append(string str)
        {
            this._sb.Append(str);

            return this;
        }
  
        public abstract T First<T>() where T : IEntity;

        public DataTable Result()
        {
            using (IDataReader rdr = this.Db.ExecuteReader(this.ToString(), this.Parameters.ToArray()))
            {
                DataTable dt = new DataTable();

                dt.Load(rdr);

                return dt;
            }
        }
 
        public List<T> ToList<T>() where T : IEntity
        {
            return this.Db.ExecuteList<T>(this) as List<T>;
        }       
        
        protected string Enclosed(string str)
        {
            if (str.IndexOf('[') == -1)
            {
                return "[" + str + "]";
            }

            return str;
        }

        public abstract int Single();
       
        public virtual int Execute()
        {
            return this.Db.ExecuteNonQuery(this.ToString(), this.Parameters.ToArray());
        }

        public abstract int ExecuteReturnId();
    }
}
