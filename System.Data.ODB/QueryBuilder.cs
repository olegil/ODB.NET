using System;
using System.Collections.Generic;
using System.Text;

namespace System.Data.ODB
{
    public abstract class QueryBuilder<T> : IQuery<T> 
        where T : IEntity
    {
        public StringBuilder SqlStr;
        private IList<IDbDataParameter> _params;

        protected IDbContext _db;

        public QueryBuilder(IDbContext db)
        {
            this._db = db;

            this.SqlStr = new StringBuilder();
            this._params = new List<IDbDataParameter>(); 
        }

        public IQuery<T> Select(string[] cols)
        { 
            this.SqlStr.Append("SELECT ");
            this.SqlStr.Append(string.Join(",", cols));
          
            return this;
        }

        public IQuery<T> From()
        {
            Type type = typeof(T);

            this.SqlStr.Append(" FROM ");
            this.SqlStr.Append(MappingHelper.GetTableName(type));

            return this;
        }

        public IQuery<T> Insert(string table)
        {
            this.SqlStr.Append("INSERT INTO ");
            this.SqlStr.Append(table);

            return this;
        }

        public IQuery<T> Update(string table)
        {
            this.SqlStr.Append("UPDATE ");
            this.SqlStr.Append(table);

            return this;
        }

        public IQuery<T> Delete()
        {
            this.SqlStr.Append("DELETE");

            return this.From();
        }

        public IQuery<T> Join<T2>() where T2 : IEntity
        {
            Type type = typeof(T2);

            this.SqlStr.Append(" JOIN ");
            this.SqlStr.Append(MappingHelper.GetTableName(type));

            return this;
        }

        public IQuery<T> LeftJoin<T2>() where T2 : IEntity
        {           
            this.SqlStr.Append(" LEFT");
        
            return this.Join<T2>();
        }

        public IQuery<T> Where(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                this.SqlStr.Append(" WHERE ");
                this.SqlStr.Append(str);
            }

            return this;
        }

        public IQuery<T> And(string str)
        {
            this.SqlStr.Append(" AND ");
            this.SqlStr.Append(str);

            return this;
        }

        public IQuery<T> Or(string str)
        {
            this.SqlStr.Append(" OR ");
            this.SqlStr.Append(str);

            return this;
        }

        public IQuery<T> Equal(string str)
        {
            this.SqlStr.Append(" = " + str);

            return this;
        }

        public IQuery<T> As(string str)
        {
            this.SqlStr.Append(" AS " + str);

            return this;
        }

        public IQuery<T> On(string str)
        {
            this.SqlStr.Append(" ON " + str);

            return this;
        }

        public IQuery<T> Symbol(string str)
        {
            this.SqlStr.Append(str);

            return this;
        }

        public IQuery<T> OrderBy(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                this.SqlStr.Append(" ORDER BY ");
                this.SqlStr.Append(str);
            }

            return this;
        }

        public IQuery<T> SortAsc()
        {
            this.SqlStr.Append(" ASC");

            return this;
        }

        public IQuery<T> SortDesc()
        {
            this.SqlStr.Append(" DESC");

            return this;
        }

        public IQuery<T> Set(string str)
        {
            this.SqlStr.Append(" SET " + str);

            return this;
        }

        public IQuery<T> Count(string str)  
        {            
            this.SqlStr.Append("SELECT COUNT(" + str + ")");
            
            return this.From();
        }

        public IQuery<T> Count()
        {
            return this.Count("*");
        }

        public IQuery<T> Eq(object val)
        {
            this.SqlStr.Append(" = ");
            this.SqlStr.Append(this.AddValue(val).ParameterName);

            return this;
        }

        public IQuery<T> NotEq(object val)
        {
            this.SqlStr.Append(" <> ");
            this.SqlStr.Append(this.AddValue(val).ParameterName);

            return this;
        }

        public IQuery<T> Gt(object val)
        {
            this.SqlStr.Append(" > " + this.AddValue(val).ParameterName);

            return this;
        }

        public IQuery<T> Lt(object val)
        {
            this.SqlStr.Append(" < " + this.AddValue(val).ParameterName);

            return this;
        }

        public IQuery<T> Gte(object val)
        {
            this.SqlStr.Append(" >= " + this.AddValue(val).ParameterName);

            return this;
        }

        public IQuery<T> Lte(object val)
        {
            this.SqlStr.Append(" <= " + this.AddValue(val).ParameterName);

            return this;
        }

        public IQuery<T> Like(string str)
        {
            this.SqlStr.Append(" LIKE " + this.AddValue("%" + str + "%").ParameterName);

            return this;
        }

        public IQuery<T> Values(string strs)
        {
            this.SqlStr.Append(" VALUES (" + strs + ")");

            return this;
        }

        public abstract IDbDataParameter AddValue(object obj);
        
        public void AddParameter(IDbDataParameter p)
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

        public int LastIndex()
        {
            return this._params.Count + 1;
        }

        public T First()
        {
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
            return this.SqlStr.ToString();
        }
    }
}
