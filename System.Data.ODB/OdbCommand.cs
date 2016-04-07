using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public abstract class OdbCommand : IOdbCommand
    {
        protected IDbContext Db;
        private int level;

        public OdbCommand(IDbContext db)
        {
            this.Db = db;
            this.level = db.Depth;
        }

        /// <summary>
        /// Insert object
        /// </summary>
        public virtual int Insert<T>(T t) where T : IEntity
        {
            if (t == null || t.IsPersisted)
            {
                return -1;
            }

            Type type = t.GetType();

            IQuery<T> query = this.Db.Query<T>();

            int n = 0;

            List<string> cols = new List<string>();
            List<string> ps = new List<string>();

            query.Table = "[" + MappingHelper.GetTableName(type) + "]";

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
                                    this.Update(entry);

                                    b = entry.Id;
                                }
                                else
                                {
                                    b = this.InsertReturnId(entry);
                                }

                                this.level++;
                            }
                        }
                    }

                    cols.Add(col.Name);

                    string pr = query.AddParameter(n, b, col.Attribute);

                    ps.Add(pr);

                    n++;
                }
            }

            query.Insert(cols.ToArray()).Values(ps.ToArray());

            return this.ExecuteNonQuery(query);
        }
 
        public abstract int InsertReturnId<T>(T t) where T : IEntity;

        /// <summary>
        /// Update object
        /// </summary>
        public virtual int Update<T>(T t) where T : IEntity
        {
            if (t == null || !t.IsPersisted)
            {
                return -1;
            }

            List<string> cols = new List<string>();

            IQuery<T> query = this.Db.Query<T>();

            query.Table = "[" + MappingHelper.GetTableName(t.GetType()) + "]";

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
                                this.Delete(col.Value as IEntity);
                            }
                            else
                            {
                                IEntity entry = col.Value as IEntity;

                                this.level--;

                                if (entry.IsPersisted)
                                {
                                    this.Update(entry);

                                    b = entry.Id;
                                }
                                else
                                {
                                    b = this.InsertReturnId(entry);
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

            return this.ExecuteNonQuery(query);
        }

        /// <summary>
        /// Delete object
        /// </summary>
        public virtual int Delete<T>(T t) where T : IEntity
        {
            if (t == null || !t.IsPersisted)
                return -1;

            IQuery<T> query = this.Db.Query<T>();

            query.Table = "[" + MappingHelper.GetTableName(t.GetType()) + "]";

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

            return this.ExecuteNonQuery(query);
        }

        private int ExecuteNonQuery(IQuery query)
        {
            return this.Db.ExecuteNonQuery(query.ToString(), query.GetParams());
        }        
    }
}
