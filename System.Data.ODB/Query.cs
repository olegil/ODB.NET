using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Data.ODB
{
    public abstract class Query<T> : IQuery<T> 
        where T : IEntity
    {
        protected StringBuilder _sb; 

        protected IDbContext _db;

        public List<IDbDataParameter> Parameters { get; set; }

        public string Table { get; set; }

        public Query(IDbContext db)
        {
            this._db = db;

            this._sb = new StringBuilder();
            this.Parameters = new List<IDbDataParameter>();
 
            this.Table = MappingHelper.GetTableName(typeof(T));
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
 
            return this.Join(MappingHelper.GetTableName(type));
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

            return this.LeftJoin(MappingHelper.GetTableName(type));
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
            string name = "p" + this.Parameters.Count; 

            IDbDataParameter p = this.BindParam(name, b, null);

            this._sb.Append(p.ParameterName);

            this.Parameters.Add(p);

            return this;
        }

        public int Create()
        {
            Type type = typeof(T);

            return this.Create(type);
        }

        public virtual int Create(string table, string[] cols)
        {
            this._sb.Clear();

            this._sb.Append("CREATE TABLE IF NOT EXISTS \"" + table + "\" (\r\n");
            this._sb.Append(string.Join(",\r\n", cols));
            this._sb.Append("\r\n);");

            return this._db.ExecuteNonQuery(this._sb.ToString());
        }

        public virtual int Create(Type type)
        {
            List<string> fields = new List<string>();

            string dbtype = "";
            string col = "";
             
            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute colAttr = MappingHelper.GetColumnAttribute(pi);

                if (colAttr != null)
                {
                    if (!colAttr.IsForeignkey)
                    {
                        dbtype = MappingHelper.DataConvert(pi.PropertyType);
                    }
                    else
                    {
                        dbtype = MappingHelper.DataConvert(typeof(long));

                        this.Create(pi.PropertyType);
                    }

                    col = this.AddColumn(pi.Name, dbtype, colAttr);

                    fields.Add(col);
                }
            }

            return this.Create(type.Name, fields.ToArray());
        }

        public abstract string AddColumn(string name, string dbtype, ColumnAttribute colAttr);
        
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

        public abstract IDbDataParameter BindParam(string name, object b, ColumnAttribute attr);
                 
        public override string ToString()
        {
            return this._sb.ToString();
        }

        public abstract T First();        

        public DataSet Result()
        {
            return this._db.ExecuteDataSet(this._sb.ToString(), this.Parameters.ToArray());
        }
 
        public List<T> ToList()
        {
            return this._db.Get<T>(this) as List<T>;
        }         

        public long ToInt()
        {
            return this._db.ExecuteScalar<long>(this._sb.ToString(), this.Parameters.ToArray());
        }
    }
}
