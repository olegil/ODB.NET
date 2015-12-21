using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Data.ODB
{
    public abstract class Query<T> : IQuery 
        where T : IEntity
    {
        public StringBuilder _sb;

        private string _table;

        protected IDbContext _db;

        public List<IDbDataParameter> Parameters { get; set; }

        private int _count = 0;

        public Query(IDbContext db)
        {
            this._db = db;

            this._sb = new StringBuilder();
            this.Parameters = new List<IDbDataParameter>();
 
            this._table = MappingHelper.GetTableName(typeof(T));
        }

        public virtual IQuery Create()
        {
            Type type = typeof(T);

            List<string> fields = new List<string>();

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute colAttr = MappingHelper.GetColumnAttribute(pi);

                if (colAttr != null)
                {
                    string name = colAttr.Name == "" ? pi.Name : colAttr.Name;
                    string dbtype = MappingHelper.DataConvert(pi.PropertyType);

                    string col = this.AddColumn(name, dbtype, colAttr);

                    fields.Add(col);
                }
            }

            return this.Create(fields.ToArray());
        }

        public IQuery Create(string[] cols)
        {
            this._sb.Append("CREATE TABLE IF NOT EXISTS \"" + this._table + "\" (\r\n");
            this._sb.Append(string.Join(",\r\n", cols));
            this._sb.Append("\r\n);");

            return this;
        }

        public abstract string AddColumn(string name, string dbtype, ColumnAttribute col);

        public virtual IQuery Drop()
        {
            this._sb.Append("DROP TABLE IF EXISTS ");
            this._sb.Append(this._table);

            return this;
        }

        public virtual IQuery Select(string[] cols)
        { 
            this._sb.Append("SELECT ");
            this._sb.Append(string.Join(",", cols));
          
            return this;
        }

        public virtual IQuery From()
        {
            this._sb.Append(" FROM ");
            this._sb.Append(this._table);

            return this;
        }

        public virtual IQuery Insert(string[] cols)
        {
            this._sb.Append("INSERT INTO " + this._table);            
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

        public virtual IQuery Update()
        {
            this._sb.Append("UPDATE ");
            this._sb.Append(this._table);

            return this;
        }

        public virtual IQuery Set(string[] cols)
        {
            this._sb.Append(" SET ");
            this._sb.Append(string.Join(", ", cols));

            return this;
        }

        public virtual IQuery Delete()
        {
            this._sb.Append("DELETE");

            return this.From();
        }

        public virtual IQuery Join<T1>() where T1 : IEntity
        {
            Type type = typeof(T1);

            this._sb.Append(" JOIN ");
            this._sb.Append(MappingHelper.GetTableName(type));

            return this;
        }

        public virtual IQuery LeftJoin<T1>() where T1 : IEntity
        {           
            this._sb.Append(" LEFT");
        
            return this.Join<T1>();
        }

        public virtual IQuery Where(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                this._sb.Append(" WHERE ");
                this._sb.Append(str);
            }

            return this;
        }

        public virtual IQuery And(string str)
        {
            this._sb.Append(" AND ");
            this._sb.Append(str);

            return this;
        }

        public virtual IQuery Or(string str)
        {
            this._sb.Append(" OR ");
            this._sb.Append(str);

            return this;
        }

        public virtual IQuery Equal(string str)
        {
            this._sb.Append(" = " + str);

            return this;
        }

        public virtual IQuery As(string str)
        {
            this._sb.Append(" AS " + str);

            return this;
        }

        public virtual IQuery On(string str)
        {
            this._sb.Append(" ON " + str);

            return this;
        }

        public virtual IQuery Append(string str)
        {
            this._sb.Append(str);

            return this;
        }

        public virtual IQuery OrderBy(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                this._sb.Append(" ORDER BY ");
                this._sb.Append(str);
            }

            return this;
        }

        public virtual IQuery SortAsc()
        {
            this._sb.Append(" ASC");

            return this;
        }

        public virtual IQuery SortDesc()
        {
            this._sb.Append(" DESC");

            return this;
        } 

        public virtual IQuery Count(string str)  
        {            
            this._sb.Append("SELECT COUNT(" + str + ")");
            
            return this.From();
        }

        public virtual IQuery Count()
        {
            return this.Count("*");
        }

        public abstract IQuery Skip(int start);

        public abstract IQuery Take(int count);

        public virtual IQuery Eq(object val)
        {
            this._sb.Append(" = ");
            
            return this.Bind(val);
        }

        public virtual IQuery NotEq(object val)
        {
            this._sb.Append(" <> ");     

            return this.Bind(val);
        }

        public virtual IQuery Gt(object val)
        {
            this._sb.Append(" > ");

            return this.Bind(val);
        }

        public virtual IQuery Lt(object val)
        {
            this._sb.Append(" < ");

            return this.Bind(val);
        }

        public virtual IQuery Gte(object val)
        {
            this._sb.Append(" >= ");

            return this.Bind(val);
        }

        public virtual IQuery Lte(object val)
        {
            this._sb.Append(" <= ");

            return this.Bind(val);
        }

        public virtual IQuery Like(string str)
        {
            this._sb.Append(" LIKE ");

            return this.Bind("%" + str + "%");
        }

        public virtual IQuery Bind(object b)
        {
            string name = "p" + this._count++;

            IDbDataParameter p = this.BindParam(name, b, null);

            this._sb.Append(p.ParameterName);
            
            return this;
        }

        public abstract IDbDataParameter BindParam(string name, object b, ColumnAttribute attr);
                 
        public override string ToString()
        {
            return this._sb.ToString();
        }

        public abstract T1 First<T1>() where T1 : IEntity;        

        public List<T1> ToList<T1>() where T1 : IEntity
        {
            return this._db.Get<T1>(this) as List<T1>;
        }
    }
}
