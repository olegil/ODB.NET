using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Data.ODB
{
    public abstract class OdbQuery<T> : IQuery<T> 
        where T : IEntity
    {
        protected StringBuilder _sb; 

        protected IDbContext _db;
               
        public List<IDbDataParameter> DbParams { get; set; }

        public string Table { get; set; }

        public OdbQuery(IDbContext db)
        {
            this._db = db;

            this._sb = new StringBuilder();           
       
            this.DbParams = new List<IDbDataParameter>();
 
            this.Table = typeof(T).Name;
        }
        
        public virtual IQuery Insert(string[] cols)
        {
            this._sb.Append("INSERT INTO " + this.Table);            
            this._sb.Append(" (");
            this._sb.Append(string.Join(", ", cols));
            this._sb.Append(")");                

            return this;
        }

        public virtual IQuery Values(string[] cols)
        {
            this._sb.Append(" VALUES (");
            this._sb.Append(string.Join(", ", cols));
            this._sb.Append(");");

            return this;
        }

        public virtual IQuery<T> Update()
        {
            this._sb.Append("UPDATE ");
            this._sb.Append(this.Table);

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

        public virtual IQuery<T> Select(string[] cols)
        {
            this._sb.Append("SELECT " + string.Join(",", cols));
        
            return this;
        }

        public virtual IQuery<T> From()
        {
            return From(this.Table);
        } 

        public virtual IQuery<T> From(string table)
        {
            this._sb.Append(" FROM ");
            this._sb.Append(table);

            return this;
        }

        public virtual IQuery<T> Join<T1>() where T1 : IEntity
        {
            Type type = typeof(T1);
 
            return this.Join(type.Name);
        }

        public virtual IQuery<T> Join(string table)
        {
            this._sb.Append(" JOIN ");
            this._sb.Append(table);

            return this;
        }

        public virtual IQuery<T> LeftJoin<T1>() where T1 : IEntity
        {
            Type type = typeof(T1);

            return this.LeftJoin(type.Name);
        }

        public virtual IQuery<T> LeftJoin(string table)
        {
            this._sb.Append(" LEFT");

            return this.Join(table);
        }

        public virtual IQuery<T> Where(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                this._sb.Append(" WHERE ");
                this._sb.Append(str);
            }

            return this;
        }

        public virtual IQuery<T> And(string str)
        {
            this._sb.Append(" AND ");
            this._sb.Append(str);

            return this;
        }

        public virtual IQuery<T> Or(string str)
        {
            this._sb.Append(" OR ");
            this._sb.Append(str);

            return this;
        }

        public virtual IQuery<T> Equal(string str)
        {
            this._sb.Append(" = " + str);

            return this;
        }

        public virtual IQuery<T> As(string str)
        {
            this._sb.Append(" AS " + str);

            return this;
        }

        public virtual IQuery<T> On(string str)
        {
            this._sb.Append(" ON " + str);

            return this;
        }

        public virtual IQuery Append(string str)
        {
            this._sb.Append(str);

            return this;
        }

        public virtual IQuery<T> OrderBy(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                this._sb.Append(" ORDER BY ");
                this._sb.Append(str);
            }

            return this;
        }

        public virtual IQuery<T> SortAsc()
        {
            this._sb.Append(" ASC");

            return this;
        }

        public virtual IQuery<T> SortDesc()
        {
            this._sb.Append(" DESC");

            return this;
        } 

        public virtual IQuery<T> Count(string str)  
        {
            string[] cols = { " COUNT(" + str + ")" };

            return this.Select(cols); 
        }

        public abstract IQuery<T> Skip(int start);

        public abstract IQuery<T> Take(int count);

        public virtual IQuery<T> Eq(object val)
        {
            this._sb.Append(" = ");
            
            return this.Bind(val);
        }

        public virtual IQuery<T> NotEq(object val)
        {
            this._sb.Append(" <> ");     

            return this.Bind(val);
        }

        public virtual IQuery<T> Gt(object val)
        {
            this._sb.Append(" > ");

            return this.Bind(val);
        }

        public virtual IQuery<T> Lt(object val)
        {
            this._sb.Append(" < ");

            return this.Bind(val);
        }

        public virtual IQuery<T> Gte(object val)
        {
            this._sb.Append(" >= ");

            return this.Bind(val);
        }

        public virtual IQuery<T> Lte(object val)
        {
            this._sb.Append(" <= ");

            return this.Bind(val);
        }

        public virtual IQuery<T> Not(string str)
        {
            this._sb.Append(" NOT ");

            return this;
        }

        public virtual IQuery<T> Like(string str)
        {
            this._sb.Append(" LIKE ");

            return this.Bind("%" + str + "%");
        }

        public virtual IQuery<T> Bind(object b)
        {
            string pa = "@p" + this.DbParams.Count; 

            this.AddParam(pa, b, null);

            this._sb.Append(pa);
                   
            return this;
        }

        public int Create()
        {
            Type type = typeof(T);

            return this.Create(type);
        }

        public virtual int Create(string table, string[] cols)
        {
            string sql = "CREATE TABLE IF NOT EXISTS \"" + table + "\" (\r\n" + string.Join(",\r\n", cols) + "\r\n);";        

            return this._db.ExecuteNonQuery(sql);
        }

        public virtual int Create(Type type)
        { 
            string dbtype = "";
            List<string> cols = new List<string>();          

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute colAttr = MappingHelper.GetColumnAttribute(pi);

                if (colAttr != null)
                {
                    if (!colAttr.IsForeignkey)
                    {
                        dbtype = this.TypeMapping(pi.PropertyType);
                    }
                    else
                    {
                        dbtype = this.TypeMapping(typeof(long));

                        this.Create(pi.PropertyType);
                    }

                    cols.Add(this.Define(pi.Name, dbtype, colAttr));                    
                }
            }

            return this.Create(type.Name, cols.ToArray());
        } 
        
        public virtual int Drop()
        {
            Type type = typeof(T);

            return this.Drop(type);
        }

        public virtual int Drop(string table)
        {
            return this._db.ExecuteNonQuery("DROP TABLE IF EXISTS " + table);
        }

        public virtual int Drop(Type type)
        { 
            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute colAttr = MappingHelper.GetColumnAttribute(pi);

                if (colAttr != null && colAttr.IsForeignkey)
                {
                    this.Drop(pi.PropertyType);
                }
            }

            return this.Drop(type.Name); 
        }

        public abstract string Define(string name, string dbtype, ColumnAttribute colAttr);

        public abstract void AddParam(string name, object b, ColumnAttribute attr);

        public IDbDataParameter[] GetParams()
        {
            return this.DbParams.ToArray();
        }

        public override string ToString()
        {
            return this._sb.ToString();
        }

        public abstract string TypeMapping(Type type);

        public abstract T First();        

        public DataSet Result()
        {
            return this._db.ExecuteDataSet(this._sb.ToString(), this.DbParams.ToArray());
        }
 
        public List<T> ToList()
        {
            return this._db.Get<T>(this) as List<T>;
        }         

        public long ToInt()
        {
            return this._db.ExecuteScalar<long>(this._sb.ToString(), this.DbParams.ToArray());
        }
    }
}
