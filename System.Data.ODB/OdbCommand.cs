using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public abstract class OdbCommand : ICommand     
    {
        protected IDbContext Db;
        protected int level;

        public OdbCommand(IDbContext db)
        {
            this.Db = db;
            this.level = db.Depth;
        }
        
        /// <summary>
        /// Create Table
        /// </summary>
        public virtual void ExecuteCreate<T>() where T : IEntity
        {
            this.Create(typeof(T));
        }

        public virtual void Create(Type type)
        { 
            List<string> cols = new List<string>();

            foreach (OdbColumn col in OdbMapping.GetColumn(type))
            { 
                if (col.Attribute.IsForeignkey)
                {   
                    this.Create(col.GetColumnType());
                }
 
                cols.Add(this.SqlDefine(col));                 
            }

            string table = OdbMapping.GetTableName(type);

            this.Create(table, cols.ToArray());
        }

        public abstract void Create(string table, string[] cols);
         
        /// <summary>
        /// Drop Table
        /// </summary>
        public virtual void ExecuteDrop<T>() where T : IEntity
        { 
            this.ExecuteDrop(typeof(T));
        }

        public virtual void ExecuteDrop(Type type)
        {
            foreach (OdbColumn col in OdbMapping.GetColumn(type))
            {
                if (col.Attribute.IsForeignkey)
                {
                    this.ExecuteDrop(col.GetColumnType());
                } 
            } 

            string table = OdbMapping.GetTableName(type);

            this.Drop(table);
        }

        public abstract void Drop(string table);

        /// <summary>
        /// Persistence object
        /// </summary>
        public virtual int ExecuteNonQuery<T>(T t) where T : IEntity
        {  
            Type type = t.GetType();

            IQuery<T> query = this.Db.CreateQuery<T>();

            //Type is IEntity
            query.Table = OdbMapping.GetTableName(type);

            int n = 0;

            List<string> cols = new List<string>();
            List<string> ps = new List<string>();

            OdbColumn ColPk = null;
             
            //begin foreach
            foreach (OdbColumn col in OdbMapping.GetColumn(type))
            {
                ColumnAttribute attr = col.Attribute;

                if (!attr.IsAuto)
                {
                    object b = col.GetValue(t);

                    if (!attr.IsForeignkey)
                    {
                        if (b == null)                           
                            b = DBNull.Value;
                    }
                    else
                    { 
                        if (this.level > 1 )
                        {
                            if (b != null)
                            {
                                IEntity entry = b as IEntity;

                                this.level--;

                                //return id
                                b = this.ExecuteNonQuery(entry);

                                this.level++;
                            }                     
                        }
                    }

                    string name = "@p" + n;

                    IDbDataParameter p = this.CreateParameter();

                    p.ParameterName = name;
                    p.Value = b;
                    p.DbType = col.GetDbType();

                    query.Parameters.Add(p);

                    ps.Add(name);
                    cols.Add("[" + col.Name + "]");

                    n++;
                }

                if (attr.IsPrimaryKey)
                {
                    ColPk = col;
                }
            }
            //end

            if (ColPk == null)
            {
                throw new OdbException("No Key.");
            }
            
            if (t.IsPersisted)
            {
                for(int i = 0; i< cols.Count; i++)
                {
                    cols[i] = cols[i] + "=" + ps[i];
                } 
            
                query.Update().Set(cols.ToArray()).Where(ColPk.Name).Eq(ColPk.GetValue(t));

                this.ExecuteUpdate(query);

                return (int)t.Id; 
            }
            else
            {
                query.Insert(cols.ToArray()).Values(ps.ToArray());

                return this.ExecuteInsert(query);
            } 
        }

        /// <summary>
        /// Insert object return id
        /// </summary>
        public abstract int ExecuteInsert(IQuery query);
         
        /// <summary>
        /// Update object
        /// </summary>
        public virtual int ExecuteUpdate(IQuery query)
        {
            return this.Execute(query);
        }

        /// <summary>
        /// Delete object
        /// </summary>
        public virtual int ExecuteDelete<T>(T t) where T : IEntity
        {
            if (t == null || !t.IsPersisted)
                return -1;

            IQuery<T> query = this.Db.CreateQuery<T>();

            Type type = t.GetType();

            query.Table = OdbMapping.GetTableName(type);

            OdbColumn colKey = null;

            foreach (OdbColumn col in OdbMapping.GetColumn(type))
            {
                if (col.Attribute.IsPrimaryKey)
                {
                    colKey = col;
                }
            }

            if (colKey == null)
            {
                throw new Exception("No key column.");
            }

            query.Delete().Where(colKey.Name).Eq(colKey.GetValue(t));

            return this.Execute(query);
        }

        public abstract IDbDataParameter CreateParameter();

        public abstract string TypeMapping(DbType dtype);

        public abstract string SqlDefine(OdbColumn col);

        protected int Execute(IQuery query)
        {
            return this.Db.ExecuteNonQuery(query.ToString(), query.Parameters.ToArray());
        }
    }
}
