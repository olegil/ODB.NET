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
            string dbtype = "";

            List<string> cols = new List<string>();

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute colAttr = MappingHelper.GetColumnAttribute(pi);

                if (!colAttr.NotMapped)
                {
                    if (!colAttr.IsForeignkey)
                    {
                        dbtype = this.TypeMapping(pi.PropertyType);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(colAttr.Name))
                            colAttr.Name = pi.Name + "Id";

                        dbtype = this.TypeMapping(typeof(long));

                        this.Create(pi.PropertyType);
                    }

                    string colName = string.IsNullOrEmpty(colAttr.Name) ? pi.Name : colAttr.Name;

                    cols.Add(this.Define(colName, dbtype, colAttr));
                }
            }

            string table = MappingHelper.GetTableName(type);

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
            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute colAttr = MappingHelper.GetColumnAttribute(pi);

                if (colAttr != null && colAttr.IsForeignkey)
                {
                    this.ExecuteDrop(pi.PropertyType);
                }
            }

            string table = MappingHelper.GetTableName(type);

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
            query.Table = MappingHelper.GetTableName(type);

            int n = 0;

            List<string> cols = new List<string>();
            List<string> ps = new List<string>();

            ColumnMapping ColPk = null;
             
            foreach (ColumnMapping col in MappingHelper.GetColumnMapping(t))
            {
                if (!col.Attribute.IsAuto)
                {
                    object b = DBNull.Value;

                    if (!col.Attribute.IsForeignkey)
                    {
                        if (col.Value != null)
                            b = col.Value;
                        else
                            b = DBNull.Value;
                    }
                    else
                    { 
                        if (this.level > 1 )
                        {
                            if (col.Value != null)
                            {
                                IEntity entry = col.Value as IEntity;

                                this.level--;

                                //return id
                                b = this.ExecuteNonQuery(entry);

                                this.level++;
                            }                     
                        }
                    }

                    string pr = query.AddParameter(n++, b);

                    ps.Add(pr);

                    cols.Add(TypeHelper.Enclosed(col.Name)); 
                }

                if (col.Attribute.IsPrimaryKey)
                {
                    ColPk = col;
                }
            }

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
            
                query.Update().Set(cols.ToArray()).Where(ColPk.Name).Eq(ColPk.Value);

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
        /// Insert object (return id)
        /// </summary>
        public abstract int ExecuteInsert(IQuery query);

        /// <summary>
        /// Update object
        /// </summary>
        public virtual void ExecuteUpdate(IQuery query)
        {
            this.Execute(query);
        } 
                
        /// <summary>
        /// Delete object
        /// </summary>
        public virtual int ExecuteDelete<T>(T t) where T : IEntity
        {
            if (t == null || !t.IsPersisted)
                return -1;

            IQuery<T> query = this.Db.CreateQuery<T>();

            query.Table = MappingHelper.GetTableName(t.GetType());

            ColumnMapping colKey = null;

            foreach (ColumnMapping col in MappingHelper.GetColumnMapping(t))
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

            query.Delete().Where(colKey.Name).Eq(colKey.Value);

            return this.Execute(query);
        }
                      
        public abstract string TypeMapping(Type type);

        public abstract string Define(string name, string dbtype, ColumnAttribute colAttr);

        protected int Execute(IQuery query)
        {
            return this.Db.ExecuteNonQuery(query.ToString(), query.GetParams());
        } 
    }
}
