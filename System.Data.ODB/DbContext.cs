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

        public abstract IQuery<T> Query<T>() where T : IEntity;
               
        #endregion

        #region ORM 

        /// <summary>
        /// Create a Table 
        /// </summary>
        public virtual int Create<T>() where T : IEntity
        {
            IQuery query = this.Query<T>().Create();

            return this.ExecuteNonQuery(query); 
        }

        /// <summary>
        /// Drop a Table 
        /// </summary>
        public virtual int Remove<T>() where T : IEntity
        {
            IQuery query = this.Query<T>().Drop();

            return this.ExecuteNonQuery(query);
        }

        /// <summary>
        /// Select from Table
        /// </summary>
        public virtual IQuery<T> From<T>() where T : IEntity
        { 
            return this.From<T>(new[] { "*" });
        }

        public virtual IQuery<T> From<T>(string[] cols) where T : IEntity
        { 
            return this.Query<T>().Select(string.Join(", ", cols)).From();
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

            EntityReader<T> edr = new EntityReader<T>(rdr);
           
            foreach (T t in edr)
            {
                list.Add(t);

                if (IsEntityTracking)
                {
                    this.DbState.Add(t.EntityId, new EntityState(t));
                }
            }
 
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

            int n = 0;

            foreach (ColumnMapping col in table.Columns)
            {
                if (!col.Attribute.IsAuto)
                {
                    IDbDataParameter p = query.BindParam("p" + n, col.Value, col.Attribute);

                    fields.Add(col.Name);

                    ps.Add(p.ParameterName);

                    n++;
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
                int n = 0;

                foreach (ColumnMapping col in table.Columns)
                {
                    IDbDataParameter pr = query.BindParam("p" + n, col.Value, col.Attribute);

                    fields.Add(col.Name + " = " + pr.ParameterName);

                    n++;
                }
            }
            else
            {
                if (this.DbState.ContainsKey(t.EntityId))
                {
                    EntityState au = this.DbState[t.EntityId];

                    int n = 0;

                    foreach (ColumnMapping col in table.Columns)
                    {
                        if (au.IsModified(col.Name, col.Value))
                        {
                            IDbDataParameter pr = query.BindParam("p" + n, col.Value, col.Attribute);

                            fields.Add(col.Name + " = " + pr.ParameterName);

                            n++;
                        }
                    }
                }               
            } 

            if (fields.Count > 0)
            {
                query.Update().Set(fields.ToArray()).Where(colKey.Name).Eq(colKey.Value);

                return this.ExecuteNonQuery(query);
            }
            
            return 0;   
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

            return this.ExecuteNonQuery(query.ToString(), query.Parameters.ToArray());          
        }

        /// <summary>
        /// Delete table
        /// </summary>
        public virtual int Clear<T>() where T : IEntity
        {
            IQuery q = this.Query<T>().Delete();

            return this.ExecuteNonQuery(q);
        }

        public virtual IQuery<T> Count<T>() where T : IEntity
        {
            return this.Query<T>().Count("*").From();
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
