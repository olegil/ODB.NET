using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Data.ODB
{
    public abstract class OdbQuery<T> : IQuery<T>
        where T : IEntity
    {
        protected StringBuilder _sb;

        public IDbContext Db { get; set; }

        public List<IDbDataParameter> Parameters { get; set; }

        public string Table { get; set; }
        public string Alias { get; set; }

        protected string _where;
        protected string _limit;
        protected string _order;
         
        public OdbQuery()
        {
            this._sb = new StringBuilder();
             
            this.Parameters = new List<IDbDataParameter>();

            this.Table = OdbMapping.GetTableName(typeof(T));

            this._where = "";
            this._limit = "";
            this._order = "";
        }

        public virtual IQuery<T> Select(string[] cols)
        {
            this._sb.Append("SELECT ");
            this._sb.Append(string.Join(",", cols));

            return this;
        }

        public virtual IQuery<T> From()
        {
            return From(this.Table);
        }

        public virtual IQuery<T> From(string table, string alias = "")
        {
            this._sb.Append(" FROM ");

            this._sb.Append(Enclosed(table));

            if (!string.IsNullOrEmpty(alias))
            {
                this.Alias = alias;

                return this.As(alias);
            }

            return this;
        }

        public virtual IQuery<T> Insert(string[] cols)
        {
            this._sb.Append("INSERT INTO ");

            this._sb.Append(Enclosed(this.Table));

            this._sb.Append(" (");    
            this._sb.Append(string.Join(", ", cols));
            this._sb.Append(")");                

            return this;
        }

        public virtual IQuery<T> Values(string[] cols)
        {
            this._sb.Append(" VALUES (");
            this._sb.Append(string.Join(", ", cols));
            this._sb.Append(")");

            return this;
        }

        public virtual IQuery<T> Update()
        {
            this._sb.Append("UPDATE ");
            this._sb.Append(Enclosed(this.Table));

            return this;
        }

        public virtual IQuery<T> Set(string[] cols)
        {
            this._sb.Append(" SET ");
            this._sb.Append(string.Join(", ", cols));

            return this;
        }

        public virtual IQuery<T> Delete()
        {
            this._sb.Append("DELETE");

            return this.From();
        }
          
        public virtual IQuery<T> Where(string str)
        { 
            this._where = this.AddAlias(str);

            return this;
        }

        public virtual IQuery<T> AddWhere(string str)
        {
            this._where += str;

            return this;
        }

        public virtual IQuery<T> And(string str)
        {
            this._where += " AND " + this.AddAlias(str); 

            return this;
        }

        public virtual IQuery<T> Or(string str)
        {
            this._where += " OR " + this.AddAlias(str);  

            return this;
        }

        public virtual IQuery<T> Equal(string str)
        {
            this._sb.Append(" = ");
            this._sb.Append(str);

            return this;
        } 

        public virtual IQuery<T> Eq(object val)
        {
            this._where += " = " + this.Add(val);

            return this;
        }

        public virtual IQuery<T> NotEq(object val)
        {
            this._where += " <> " + this.Add(val);

            return this;
        }

        public virtual IQuery<T> Gt(object val)
        {
            this._where += " > " + this.Add(val);

            return this;
        }

        public virtual IQuery<T> Lt(object val)
        {
            this._where += " < " + this.Add(val);

            return this;
        }

        public virtual IQuery<T> Gte(object val)
        {
            this._where += " >= " + this.Add(val);
         
            return this;
        }

        public virtual IQuery<T> Lte(object val)
        {
            this._where += " <= " + this.Add(val);
             
            return this;
        } 

        public virtual IQuery<T> Like(string str)
        {
            this._where += " LIKE " + this.Add("%" + str + "%");
             
            return this;
        }

        public virtual IQuery<T> OrderBy(string str)
        {
            this._order = this.AddAlias(str);
           
            return this;
        }

        public virtual IQuery<T> SortAsc()
        {
            this._order += " ASC";

            return this;
        }

        public virtual IQuery<T> SortDesc()
        {
            this._order += " DESC";
      
            return this;
        }

        public virtual IQuery<T> Join<T1>() where T1 : IEntity
        {
            Type type = typeof(T1);

            return this.Join(OdbMapping.GetTableName(type));
        }

        public virtual IQuery<T> Join(string table)
        {
            this._sb.Append(" JOIN ");
            this._sb.Append(Enclosed(table));

            return this;
        }

        public virtual IQuery<T> LeftJoin<T1>() where T1 : IEntity
        {
            Type type = typeof(T1);

            return this.LeftJoin(OdbMapping.GetTableName(type));
        }

        public virtual IQuery<T> LeftJoin(string table)
        {
            this._sb.Append(" LEFT");

            return this.Join(table);
        }

        public virtual IQuery<T> As(string str)
        {
            this._sb.Append(" AS ");
            this._sb.Append(Enclosed(str));

            return this;
        }

        public virtual IQuery<T> On(string str)
        {
            this._sb.Append(" ON ");
            this._sb.Append(str);

            return this;
        }

        public IQuery<T> Not()
        {
            this._where += " NOT ";

            return this;
        }

        public virtual IQuery<T> Count(string str)  
        {
            string[] cols = { " COUNT(" + str + ")" };

            return this.Select(cols); 
        }

        public abstract IQuery<T> Skip(int start);

        public abstract IQuery<T> Take(int count);
         
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
            if (!string.IsNullOrEmpty(this.Alias))
            {
                return Enclosed(this.Alias) + "." + Enclosed(str);
            }
 
            return Enclosed(str);
        }

        public IQuery Append(string str)
        {
            this._sb.Append(str);

            return this;
        }
  
        public abstract T First();        

        public DataSet Result()
        {
            return this.Db.ExecuteDataSet(this._sb.ToString(), this.Parameters.ToArray());
        }
 
        public List<T> ToList()
        {
            return this.Db.Get<T>(this) as List<T>;
        }       
          
        public T1 Single<T1>()
        {
            return this.Db.ExecuteScalar<T1>(this._sb.ToString(), this.Parameters.ToArray());
        }

        private string Enclosed(string str)
        {
            if (str.IndexOf('[') == -1)
            {
                return "[" + str + "]";
            }

            return str;
        }

        public void Reset()
        {
            this._sb.Length = 0;
            this.Parameters.Clear();

            this._where = "";
            this._order = "";
            this._limit = "";
        }
    }
}
