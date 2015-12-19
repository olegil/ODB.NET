using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.ODB
{
    public abstract class QueryBuilder<T> : IQuery<T> 
        where T : IEntity
    {
        public StringBuilder _sb;

        private string _table;

        private IList<IDbDataParameter> _params;

        protected IDbContext _db;

        public QueryBuilder(IDbContext db)
        {
            this._db = db;

            this._sb = new StringBuilder();
            this._params = new List<IDbDataParameter>();

            Type type = typeof(T);

            this._table = MappingHelper.GetTableName(type);
        }

        public virtual IQuery<T> Create(string[] cols)
        {
            this._sb.Append("CREATE TABLE IF NOT EXISTS \"" + this._table + "\" (\r\n"); 
            this._sb.Append(string.Join(",\r\n", cols));
            this._sb.Append("\r\n);");

            return this;
        }

        public virtual IQuery<T> Drop()
        {
            this._sb.Append("DROP TABLE IF EXISTS ");
            this._sb.Append(this._table);

            return this;
        }

        public virtual IQuery<T> Select(string[] cols)
        { 
            this._sb.Append("SELECT ");
            this._sb.Append(string.Join(",", cols));
          
            return this;
        }

        public virtual IQuery<T> From()
        {
            this._sb.Append(" FROM ");
            this._sb.Append(this._table);

            return this;
        }

        public virtual IQuery<T> Insert(string[] cols)
        {
            this._sb.Append("INSERT INTO " + this._table);            
            this._sb.Append(" (");
            this._sb.Append(string.Join(", ", cols));
            this._sb.Append(")");                

            return this;
        }

        public virtual IQuery<T> Values(string[] cols)
        {
            this._sb.Append(" VALUES (");
            this._sb.Append(string.Join(", ", cols));
            this._sb.Append(");");

            return this;
        }

        public virtual IQuery<T> Update()
        {
            this._sb.Append("UPDATE ");
            this._sb.Append(this._table);

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

        public virtual IQuery<T> Join<T2>() where T2 : IEntity
        {
            Type type = typeof(T2);

            this._sb.Append(" JOIN ");
            this._sb.Append(MappingHelper.GetTableName(type));

            return this;
        }

        public virtual IQuery<T> LeftJoin<T2>() where T2 : IEntity
        {           
            this._sb.Append(" LEFT");
        
            return this.Join<T2>();
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

        public virtual IQuery<T> Append(string str)
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
            this._sb.Append("SELECT COUNT(" + str + ")");
            
            return this.From();
        }

        public virtual IQuery<T> Count()
        {
            return this.Count("*");
        }

        public abstract IQuery<T> Skip(int start);

        public abstract IQuery<T> Take(int count);

        public virtual IQuery<T> Eq(object val)
        {
            this._sb.Append(" = ");
            this._sb.Append(this.AddValue(val).ParameterName);

            return this;
        }

        public virtual IQuery<T> NotEq(object val)
        {
            this._sb.Append(" <> ");
            this._sb.Append(this.AddValue(val).ParameterName);

            return this;
        }

        public virtual IQuery<T> Gt(object val)
        {
            this._sb.Append(" > " + this.AddValue(val).ParameterName);

            return this;
        }

        public virtual IQuery<T> Lt(object val)
        {
            this._sb.Append(" < " + this.AddValue(val).ParameterName);

            return this;
        }

        public virtual IQuery<T> Gte(object val)
        {
            this._sb.Append(" >= " + this.AddValue(val).ParameterName);

            return this;
        }

        public virtual IQuery<T> Lte(object val)
        {
            this._sb.Append(" <= " + this.AddValue(val).ParameterName);

            return this;
        }

        public virtual IQuery<T> Like(string str)
        {
            this._sb.Append(" LIKE " + this.AddValue("%" + str + "%").ParameterName);

            return this;
        } 

        public abstract IDbDataParameter AddValue(object obj);
        
        public virtual void AddParameter(IDbDataParameter p)
        {
            this._params.Add(p);
        }

        public IDbDataParameter[] GetParameters()
        {
            IDbDataParameter[] retval = new IDbDataParameter[this._params.Count];

            for (int i = 0; i < retval.Length; i++)
                retval[i] = this._params[i];

            return retval;
        } 

        public T First()
        {
            this.Skip(0).Take(1);

            IList<T> list = this._db.Get(this);

            if (list.Count > 0)
                return list[0];

            return default(T);
        } 

        public List<T> ToList()
        {
            return this._db.Get(this) as List<T>;
        }

        public DataTable GetTable()
        {
            return this._db.ExecuteDataSet(this).Tables[0];
        }
         
        public int ExecuteCommand()
        {
            return this._db.ExecuteNonQuery(this);
        }
            
        public override string ToString()
        {
            return this._sb.ToString();
        }          
    }
}
