using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Data.ODB
{
    public abstract class DbContext : IDbContext, IDisposable       
    {         
        public int Depth { get; set; }
 
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Close();
                }
            }

            this.disposed = true;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        } 

        public IDbConnection Connection { get; private set; }

        private IDbTransaction _trans { get; set; }

        private bool _inTrans;
        public bool InTransaction { get { return this._inTrans; } }

        public DbContext(IDbConnection DbConnection)
        {
            this.Connection = DbConnection;

            this.Depth = 1;            
        }

        public void Close()
        {
            if (this._trans != null)
            {
                this._trans.Dispose();

                this._trans = null;
            }

            if (this.Connection != null)
            {
                if (this.Connection.State == ConnectionState.Open)
                    this.Connection.Close();
            }               
        }
 
        #region Transaction

        public void StartTrans()
        {
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();

            this._trans = this.Connection.BeginTransaction();

            this._inTrans = true;
        }

        public void CommitTrans()
        { 
            try
            {
                this._trans.Commit();             
                    
                this._inTrans = false;
            }
            catch
            {
                throw;
            }                            
        }

        public void RollBack()
        {
            try
            {
                this._trans.Rollback();

                this._inTrans = false;
            }
            catch
            {
                throw;
            }   
        }

        public abstract IQuery<T> Query<T>() where T : IEntity;
               
        #endregion

        #region ORM 

        /// <summary>
        /// Create a Table 
        /// </summary>
        public virtual int Create<T>(bool isCascade) where T : IEntity
        { 
            Type type = typeof(T);

            return this.Create(type, isCascade);
        }

        private int Create(Type type, bool isCascade)
        {
            List<string> fields = new List<string>();

            string name = "";
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

                        if (isCascade)
                        {
                            this.Create(pi.PropertyType, isCascade);
                        }
                    }

                    name = colAttr.Name == "" ? pi.Name : colAttr.Name;

                    col = this.AddColumn(name, dbtype, colAttr);

                    fields.Add(col);
                }
            }

            return this.Create(type.Name, fields.ToArray());
        }

        private int Create(string table, string[] cols)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("CREATE TABLE IF NOT EXISTS \"" + table + "\" (\r\n");
            sb.Append(string.Join(",\r\n", cols));
            sb.Append("\r\n);");

            return this.ExecuteNonQuery(sb.ToString());
        }

        public abstract string AddColumn(string name, string dbtype, ColumnAttribute colAttr);         

        /// <summary>
        /// Drop a Table 
        /// </summary>
        public virtual int Remove<T>(bool isCascade) where T : IEntity
        {
            Type type = typeof(T);

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute colAttr = MappingHelper.GetColumnAttribute(pi);

                if (colAttr != null && colAttr.IsForeignkey)
                {
                    if (isCascade)
                    {
                        this.Drop(pi.PropertyType);
                    }
                }
            }

            return this.Drop(type);
        }

        private int Drop(Type type)
        {
            string sql = "DROP TABLE IF EXISTS " + type.Name;

            return this.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// Select from Table
        /// </summary>
        public virtual IQuery<T> Get<T>() where T : IEntity
        { 
            TableSelector tableSel = new TableSelector();
            tableSel.Level = this.Depth;

            Type type = typeof(T);
            tableSel.Find(type);

            IQuery<T> q = this.Query<T>().Select(string.Join(", ", tableSel.Colums.ToArray())).From(type.Name).As("T0");

            foreach(string s in tableSel.Tables)
            {
                q.Append(s);
            }

            return q;
        }
 
        /// <summary>
        /// Get query result
        /// </summary>
        public virtual IList<T> Get<T>(IQuery q) where T : IEntity
        {
            using (IDataReader rdr = this.ExecuteReader(q))
            {
                return this.Get<T>(rdr);               
            }
        } 

        public virtual IList<T> Get<T>(IDataReader rdr) where T : IEntity
        {           
            IList<T> list = new List<T>();

            EntityReader<T> edr = new EntityReader<T>(rdr, this.Depth);
           
            foreach (T t in edr)
            {
                list.Add(t);            
            }
 
            return list;
        }

        public int Store<T>(T t) where T : IEntity
        {
            if (t.IsPersisted)
                return this.Update(t);

            return this.Insert(t);
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
             
            List<string> fields = new List<string>();
            List<string> ps = new List<string>();

            TableMapping table = MappingHelper.Create(t);
          
            IQuery<T> query = this.Query<T>();

            query.Table = table.Name;

            int n = 0;
                      
            IDbDataParameter pr;

            Type type = t.GetType();

            foreach (ColumnMapping col in table.Columns)
            {
                if (!col.Attribute.IsAuto)
                { 
                    if (!col.Attribute.IsForeignkey)
                    {
                        pr = query.BindParam("p" + n, col.Value, col.Attribute);
                    }
                    else
                    { 
                        if (col.Value != null)
                        {
                            IEntity entry = col.Value as IEntity;

                            if (entry.IsPersisted)
                            {                                
                                pr = query.BindParam("p" + n, entry.Id, col.Attribute);
                            }
                            else
                            {
                                long i = this.InsertReturnId(entry);

                                pr = query.BindParam("p" + n, i, col.Attribute);
                            }
                        }
                        else
                        {
                            pr = query.BindParam("p" + n, null, col.Attribute);
                        }
                    }

                    fields.Add(col.Name);

                    ps.Add(pr.ParameterName);

                    query.Parameters.Add(pr);      

                    n++;
                }                           
            }

            query.Insert(fields.ToArray()).Values(ps.ToArray());         

            return this.ExecuteNonQuery(query); 
        }

        public abstract long InsertReturnId<T>(T t) where T : IEntity;
        
        /// <summary>
        /// Update object
        /// </summary>
        public virtual int Update<T>(T t) where T : IEntity
        {
            if (t == null || !t.IsPersisted)
            {
                return -1;
            }

            TableMapping table = MappingHelper.Create(t);

            ColumnMapping colKey = table.PrimaryKey;

            if (colKey == null)
                throw new Exception("No key column.");

            List<string> fields = new List<string>();

            IQuery<T> query = this.Query<T>();

            query.Table = table.Name;
           
            int n = 0;
           
            IDbDataParameter pr;

            foreach (ColumnMapping col in table.Columns)
            {
                if (!col.Attribute.IsPrimaryKey && !col.Attribute.IsAuto)
                { 
                    if (!col.Attribute.IsForeignkey) 
                    {
                        pr = query.BindParam("p" + n, col.Value, col.Attribute);
                    }
                    else
                    {                    
                        IEntity entry = col.Value as IEntity;
 
                        if (entry == null)
                        {
                            this.Delete(col.Value as IEntity);

                            pr = query.BindParam("p" + n, null, col.Attribute);
                        }
                        else
                        {
                            if (entry.IsPersisted)
                            {
                                this.Update(entry);

                                pr = query.BindParam("p" + n, entry.Id, col.Attribute);
                            }
                            else
                            {
                                long i = this.InsertReturnId(entry);

                                pr = query.BindParam("p" + n, i, col.Attribute);
                            }
                        }
                    }                   
 
                    fields.Add(col.Name + " = " + pr.ParameterName);

                    query.Parameters.Add(pr);

                    n++;
                } 
            }
          
            query.Update().Set(fields.ToArray()).Where(colKey.Name).Eq(colKey.Value);

            return this.ExecuteNonQuery(query);            
        }

        /// <summary>
        /// Delete object
        /// </summary>
        public virtual int Delete<T>(T t) where T : IEntity
        {
            if (t == null || !t.IsPersisted)
                return -1;

            TableMapping table = MappingHelper.Create(t);

            ColumnMapping colKey = table.PrimaryKey;

            if (colKey == null)
            {
                throw new Exception("No key column.");
            }

            IQuery<T> query = this.Query<T>();

            query.Table = table.Name;
            
            query.Delete().Where(colKey.Name).Eq(colKey.Value);
            
            return this.ExecuteNonQuery(query.ToString(), query.Parameters.ToArray());          
        }

        /// <summary>
        /// Delete all table data
        /// </summary>
        public virtual int Clear<T>() where T : IEntity
        {
            IQuery q = this.Query<T>().Delete();

            return this.ExecuteNonQuery(q);
        }

        #endregion

        #region Data Access  

        public virtual DataSet ExecuteDataSet(IQuery query)
        {
            return this.ExecuteDataSet(query.ToString(), query.Parameters.ToArray());
        }

        public abstract DataSet ExecuteDataSet(string sql, params IDbDataParameter[] commandParameters);

        public virtual IDataReader ExecuteReader(IQuery query)
        {
            return this.ExecuteReader(query.ToString(), query.Parameters.ToArray());
        }

        public IDataReader ExecuteReader(string sql, params IDbDataParameter[] commandParameters)
        {
            //create a command 
            IDbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, sql, commandParameters);
         
            try
            {
                IDataReader rdr = cmd.ExecuteReader();

                cmd.Parameters.Clear();

                return rdr;
            }
            catch
            {
                this.Connection.Close();

                throw;
            }
        }

        public T ExecuteScalar<T>(IQuery query)
        {
            return this.ExecuteScalar<T>(query.ToString(), query.Parameters.ToArray());
        }

        public T ExecuteScalar<T>(string sql, params IDbDataParameter[] commandParameters)
        {
            //create a command 
            IDbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, sql, commandParameters);

            //execute the command & return the results
            try
            {
                T retval = (T)cmd.ExecuteScalar();

                cmd.Parameters.Clear();

                return retval;
            }
            catch
            {
                this.Connection.Close();

                throw;
            }           
        }

        public virtual int ExecuteNonQuery(IQuery query)
        {
            return this.ExecuteNonQuery(query.ToString(), query.Parameters.ToArray());
        } 

        public int ExecuteNonQuery(string sql, params IDbDataParameter[] commandParameters)
        {
            //create a command
            IDbCommand cmd = this.Connection.CreateCommand();
          
            SetCommand(cmd, sql, commandParameters);

            int n = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
             
            return n;
        }

        protected void SetCommand(IDbCommand cmd, string cmdText, IDbDataParameter[] commandParameters)
        {
            //Open the connection 
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();
         
            if (this._inTrans && this._trans != null)
                cmd.Transaction = this._trans;

            cmd.Connection = this.Connection;
            cmd.CommandText = cmdText;
            cmd.CommandType = CommandType.Text;

            // Bind the parameters passed in
            if (commandParameters != null)
            {
                foreach (IDbDataParameter parm in commandParameters)
                    cmd.Parameters.Add(parm);
            }

            return;
        }       

        #endregion
    }
}
