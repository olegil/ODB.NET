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
        public string Alias { get; set; }

        public OdbQuery(IDbContext db)
        {
            this._sb = new StringBuilder();

            this._db = db;
            this.DbParams = new List<IDbDataParameter>();
 
            this.Table = typeof(T).Name;
            this.Alias = "";
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

        public virtual IQuery Insert(string[] cols)
        {
            this._sb.Append("INSERT INTO ");

            this._sb.Append(Enclosed(this.Table));

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
         
        public virtual IQuery<T> Join<T1>() where T1 : IEntity
        {
            Type type = typeof(T1);
 
            return this.Join(MappingHelper.GetTableName(type));
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

            return this.LeftJoin(MappingHelper.GetTableName(type));
        }

        public virtual IQuery<T> LeftJoin(string table)
        {
            this._sb.Append(" LEFT");

            return this.Join(table);
        }

        public virtual IQuery<T> Where(string str)
        { 
            this._sb.Append(" WHERE ");

            return this.AddAlias(str);
        }

        public virtual IQuery<T> And(string str)
        {
            this._sb.Append(" AND ");

            return this.AddAlias(str);
        }

        public virtual IQuery<T> Or(string str)
        {
            this._sb.Append(" OR ");

            return this.AddAlias(str);
        }

        public virtual IQuery<T> Equal(string str)
        {
            this._sb.Append(" = '");
            this._sb.Append(str);
            this._sb.Append("'");

            return this;
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

        public virtual IQuery<T> OrderBy(string str)
        {
            this._sb.Append(" ORDER BY ");
            this._sb.Append(Enclosed(str));

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
            int index = this.DbParams.Count;

            string p = this.AddParameter(index, b, null);

            this._sb.Append(p);
                   
            return this;
        }

        public virtual IQuery<T> AddAlias(string str)
        {
            if (!string.IsNullOrEmpty(this.Alias))
            {
                this._sb.Append(Enclosed(this.Alias) + ".");
            }

            this._sb.Append(Enclosed(str));

            return this;
        }

        public IQuery AddString(string str)
        {
            this._sb.Append(str);

            return this;
        }

        public abstract string AddParameter(int index, object b, ColumnAttribute attr);

        public IDbDataParameter[] GetParams()
        {
            return this.DbParams.ToArray();
        }

        public override string ToString()
        {
            return this._sb.ToString();
        }
 
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

        public static string Enclosed(string str)
        {
            if (str.IndexOf('[') == -1)
            {
                return "[" + str + "]";
            }

            return str;
        } 
    }
}
