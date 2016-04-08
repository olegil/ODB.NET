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
        private int level;

        public OdbCommand(IDbContext db)
        {
            this.Db = db;
            this.level = db.Depth;
        }

        public abstract int ExecuteCreate<T>() where T : IEntity;

        public virtual int Create(string table, string[] cols)
        {
            string sql = "CREATE TABLE IF NOT EXISTS \"" + table + "\" (\r\n" + string.Join(",\r\n", cols) + "\r\n);";

            return this.Db.ExecuteNonQuery(sql);
        }

        public virtual int ExecuteDrop<T>() where T : IEntity
        { 
            return this.ExecuteDrop(typeof(T));
        }

        public virtual int ExecuteDrop(Type type)
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

            return this.Drop(table);
        }

        public virtual int Drop(string table)
        {
            return this.Db.ExecuteNonQuery("DROP TABLE IF EXISTS \"" + table + "\"");
        } 

        /// <summary>
        /// Insert object
        /// </summary>
        public virtual int ExecuteInsert<T>(T t) where T : IEntity
        {
            if (t == null || t.IsPersisted)
            {
                return -1;
            }

            Type type = t.GetType();

            IQuery<T> query = this.Db.CreateQuery<T>();

            query.Table = MappingHelper.GetTableName(type); 

            int n = 0;

            List<string> cols = new List<string>();
            List<string> ps = new List<string>();
 
            foreach (ColumnMapping col in MappingHelper.GetColumnMapping(t))
            {
                if (!col.Attribute.IsAuto)
                {
                    object b = null;

                    if (!col.Attribute.IsForeignkey)
                    {
                        b = col.Value;
                    }
                    else
                    {
                        if (this.level > 1)
                        {
                            if (col.Value != null)
                            {
                                IEntity entry = col.Value as IEntity;

                                this.level--;

                                if (entry.IsPersisted)
                                {
                                    this.ExecuteUpdate(entry);

                                    b = entry.Id;
                                }
                                else
                                {
                                    b = this.ExecuteInsertReturnId(entry);
                                }

                                this.level++;
                            }
                        }
                    }
                    
                    cols.Add("[" + col.Name + "]");

                    string pr = query.AddParameter(n, b, col.Attribute);

                    ps.Add(pr);

                    n++;
                }
            }

            query.Insert(cols.ToArray()).Values(ps.ToArray());

            return this.Execute(query);
        }
 
        public abstract int ExecuteInsertReturnId<T>(T t) where T : IEntity;

        /// <summary>
        /// Update object
        /// </summary>
        public virtual int ExecuteUpdate<T>(T t) where T : IEntity
        {
            if (t == null || !t.IsPersisted)
            {
                return -1;
            }

            List<string> cols = new List<string>();

            IQuery<T> query = this.Db.CreateQuery<T>();

            query.Table = MappingHelper.GetTableName(t.GetType());

            int n = 0;

            ColumnMapping colKey = null;

            foreach (ColumnMapping col in MappingHelper.GetColumnMapping(t))
            {
                if (!col.Attribute.IsPrimaryKey && !col.Attribute.IsAuto)
                {
                    object b = null;

                    if (!col.Attribute.IsForeignkey)
                    {
                        b = col.Value;
                    }
                    else
                    {
                        if (this.level > 1)
                        {
                            if (col.Value == null)
                            {
                                this.ExecuteDelete(col.Value as IEntity);
                            }
                            else
                            {
                                IEntity entry = col.Value as IEntity;

                                this.level--;

                                if (entry.IsPersisted)
                                {
                                    this.ExecuteUpdate(entry);

                                    b = entry.Id;
                                }
                                else
                                {
                                    b = this.ExecuteInsertReturnId(entry);
                                }

                                this.level++;
                            }
                        }
                    }

                    if (b != null)
                    {
                        string pr = query.AddParameter(n, b, col.Attribute);

                        cols.Add(string.Format("{0} = {1}", col.Name, pr));

                        n++;
                    }
                }
                else
                {
                    colKey = col;
                }
            }

            if (colKey == null)
            {
                throw new Exception("No key column.");
            }

            query.Update().Set(cols.ToArray()).Where(colKey.Name).Eq(colKey.Value);

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

        private int Execute(IQuery query)
        {
            return this.Db.ExecuteNonQuery(query.ToString(), query.GetParams());
        }        
    }
}
