using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Data.ODB
{
    public abstract class DbContext : IDbContext, IDisposable       
    {         
        public bool IsEntityTracking { get; set; }
         
        public Dictionary<string, EntityState> DbState { get; private set; }

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

            this.IsEntityTracking = false;

            this.DbState = new Dictionary<string, EntityState>();
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

        #endregion

        #region ORM 

        /// <summary>
        /// Create a Table 
        /// </summary>
        public virtual int Create<T>() where T : IEntity
        {
            IQuery<T> query = this.Query<T>().Create();

            return this.ExecuteNonQuery(query); 
        }

        /// <summary>
        /// Drop a Table 
        /// </summary>
        public virtual int Drop<T>() where T : IEntity
        {
            IQuery<T> query = this.Query<T>().Drop();

            return this.ExecuteNonQuery(query);
        }

        /// <summary>
        /// Select from Table
        /// </summary>
        public virtual IQuery<T> Table<T>() where T : IEntity
        { 
            return this.Select<T>(new[] { "*" });
        }

        public virtual IQuery<T> Select<T>(string[] cols) where T : IEntity
        { 
            return this.Query<T>().Select(cols).From();
        }

        /// <summary>
        /// Get query result
        /// </summary>
        public virtual IList<T> Get<T>(IQuery<T> q) where T : IEntity
        {
            using (IDataReader rdr = this.ExecuteReader(q))
            {
                return this.Get<T>(rdr);               
            }
        }

        public virtual IList<T> Get<T>(IDataReader rdr) where T : IEntity
        {
            IList<T> list = new List<T>();

            string colName = "";

            PropertyInfo[] propertys = typeof(T).GetProperties();
             
            while (rdr.Read())
            {
                T instance = Activator.CreateInstance<T>(); 

                foreach (PropertyInfo pi in propertys)
                {
                    ColumnAttribute attr = MappingHelper.GetColumnAttribute(pi);

                    if (attr != null)
                    {
                        colName = string.IsNullOrEmpty(attr.Name) ? pi.Name : attr.Name;

                        object value = rdr[colName] == DBNull.Value ? null : rdr[colName];
 
                        pi.SetValue(instance, value, null);                         
                    }

                    if (pi.Name == "IsPersisted")
                    {
                        pi.SetValue(instance, true, null);
                    }
                }
            
                list.Add(instance); 

                if (IsEntityTracking)
                {
                    this.DbState.Add(instance.EntityId, new EntityState(instance));
                }
            }

            rdr.Close();
            
            return list;
        }

        /// <summary>
        /// Insert object
        /// </summary>
        public virtual int Insert<T>(T t) where T : IEntity
        {
            if (t == null)
            {
                throw new Exception("No object to insert.");
            }
             
            List<string> fields = new List<string>();
            List<string> ps = new List<string>();

            TableMapping table = MappingHelper.Create(t);

            IQuery<T> query = this.Query<T>();

            foreach (ColumnMapping col in table.Columns)
            {
                if (!col.Attribute.IsAuto)
                {
                    IDbDataParameter pr = this.CreateParameter(col);

                    query.AddParameter(pr);

                    fields.Add(col.Name);
                    ps.Add(pr.ParameterName);
                }                
            }

            query.Insert(fields.ToArray()).Values(ps.ToArray());         

            return this.ExecuteNonQuery(query); 
        }

        /// <summary>
        /// Update object
        /// </summary>
        public virtual int Update<T>(T t) where T : IEntity
        {
            if (t == null || !t.IsPersisted)
            {
                throw new Exception("No object to update.");
            }

            TableMapping table = MappingHelper.Create(t);

            ColumnMapping colKey = table.PrimaryKey;

            if (colKey == null)
                throw new Exception("No key column.");

            List<string> fields = new List<string>();

            IQuery<T> query = this.Query<T>();

            if (!this.IsEntityTracking)
            {
                foreach (ColumnMapping col in table.Columns)
                {
                    IDbDataParameter pr = this.CreateParameter(col);

                    query.AddParameter(pr);

                    fields.Add(col.Name + " = " + pr.ParameterName);
                }
            }
            else
            {
                if (this.DbState.ContainsKey(t.EntityId))
                {
                    EntityState au = this.DbState[t.EntityId];

                    foreach (ColumnMapping col in table.Columns)
                    {
                        if (au.IsModified(col.Name, col.Value))
                        {
                            IDbDataParameter pr = this.CreateParameter(col);

                            query.AddParameter(pr);

                            fields.Add(col.Name + " = " + pr.ParameterName);
                        }
                    }
                }               
            }

            int n = 0;

            if (fields.Count > 0)
            {
                query.Update().Set(fields.ToArray()).Where(colKey.Name).Eq(colKey.Value);

                return this.ExecuteNonQuery(query);
            }
            
            return n;   
        }

        /// <summary>
        /// Delete object
        /// </summary>
        public virtual int Delete<T>(T t) where T : IEntity
        {
            if (t == null || !t.IsPersisted)
                return 0;

            TableMapping table = MappingHelper.Create(t);

            ColumnMapping colKey = table.PrimaryKey;

            if (colKey == null)
            {
                throw new Exception("No key column.");
            }

            IQuery<T> query = this.Query<T>().Delete().Where(colKey.Name).Eq(colKey.Value);

            return this.ExecuteNonQuery(query.ToString(), query.GetParameters());          
        }

        /// <summary>
        /// Delete table
        /// </summary>
        public virtual int Clear<T>() where T : IEntity
        {
            IQuery<T> q = this.Query<T>().Delete();

            return this.ExecuteNonQuery(q);
        }

        public virtual IQuery<T> Count<T>() where T : IEntity
        {
            return this.Query<T>().Count();
        }

        public abstract IQuery<T> Query<T>() where T : IEntity;
        public abstract IDbDataParameter CreateParameter(ColumnMapping col);

        #endregion

        #region Data Access  

        public virtual DataSet ExecuteDataSet<T>(IQuery<T> query) where T : IEntity
        {
            return this.ExecuteDataSet(query.ToString(), query.GetParameters());
        }

        public abstract DataSet ExecuteDataSet(string sql, params IDbDataParameter[] commandParameters);

        public virtual IDataReader ExecuteReader<T>(IQuery<T> query) where T : IEntity
        {
            return this.ExecuteReader(query.ToString(), query.GetParameters());
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

        public T ExecuteScalar<T>(string sql, params IDbDataParameter[] commandParameters)
        {
            //create a command 
            IDbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, sql, commandParameters);

            //execute the command & return the results
            T retval = (T)cmd.ExecuteScalar();
                     
            cmd.Parameters.Clear();

            return retval;
        }

        public virtual int ExecuteNonQuery<T>(IQuery<T> query) where T : IEntity
        {
            return this.ExecuteNonQuery(query.ToString(), query.GetParameters());
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
