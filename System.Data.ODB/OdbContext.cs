using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Data.ODB
{
    public abstract class OdbContext : IDbContext, IDisposable       
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

        public IDbConnection Connection { get; set; }

        public IDbTransaction OdbTransaction { get; set; }

        private bool _inTrans;
        public bool InTransaction { get { return this._inTrans; } }

        public OdbContext(IDbConnection DbConnection)
        {
            this.Connection = DbConnection;

            this.Depth = 1;            
        }

        public void Close()
        {
            if (this.OdbTransaction != null)
            {
                this.OdbTransaction.Dispose();

                this.OdbTransaction = null;
            }

            if (this.Connection != null)
            {
                if (this.Connection.State == ConnectionState.Open)
                    this.Connection.Close();

                this.Connection = null;
            }               
        } 

        #region Transaction

        public void StartTrans()
        {
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();

            this.OdbTransaction = this.Connection.BeginTransaction();

            this._inTrans = true;
        }

        public void CommitTrans()
        { 
            try
            {
                this.OdbTransaction.Commit();             
                    
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
                this.OdbTransaction.Rollback();

                this._inTrans = false;
            }
            catch
            {
                throw;
            }   
        }

        #endregion

        #region ORM 

        public abstract IQuery<T> CreateQuery<T>() where T : IEntity;

        public abstract IQuery<T> CreateQuery<T>(string sql) where T : IEntity;       

        public IQuery<T> Query<T>() where T : IEntity
        {
            Type type = typeof(T);

            OdbDiagram dg = new OdbDiagram(this.Depth);

            dg.Analyze(type);
            
            OdbTable table = dg.Table[0];
                        
            IQuery<T> q = this.CreateQuery<T>().Select(dg.Colums).From(table.Name, table.Alias);

            int n = 1;
                        
            foreach (KeyValuePair<string, string> tc in dg.ForigeKey)
            {
                OdbTable tab = dg.Table[n++];

                q.LeftJoin(tab.Name).As(tab.Alias).On(tc.Key).Equal(tc.Value);
            }

            return q;
        }

        /// <summary>
        /// Create Table 
        /// </summary>
        /// 
        public void Create<T>() where T : IEntity
        {
            ICommand cmd = this.CreateCommand();

            cmd.ExecuteCreate<T>();
        }
         
        /// <summary>
        /// Drop Table 
        /// </summary>
        public virtual void Remove<T>() where T : IEntity
        {
            ICommand cmd = this.CreateCommand();

            cmd.ExecuteDrop<T>();
        }
                  
        /// <summary>
        /// Get query result
        /// </summary>
        public virtual IList<T> Get<T>() where T : IEntity
        {  
            IQuery<T> q = this.Query<T>();
  
            return this.Get<T>(q);
        }
  
        public virtual IList<T> Get<T>(IQuery q) where T : IEntity
        {
            using (IDataReader rdr = this.ExecuteReader(q))
            {
                IList<T> list = new List<T>();

                using (OdbReader<T> edr = new OdbReader<T>(rdr, this.Depth))
                {
                    foreach (T t in edr)
                    {
                        list.Add(t);
                    } 
                }

                return list;             
            }
        }

        public virtual int Count<T>(string str) where T : IEntity
        {
            IQuery<T> q = this.CreateQuery<T>().Count(str).From();

            return this.ExecuteScalar<int>(q);
        }

        public virtual int Count<T>() where T : IEntity
        {
            return this.Count<T>("Id");
        }

        /// <summary>
        /// Insert object
        /// </summary>
        public int Insert(IEntity t)
        {
            if (!t.IsPersisted)
            {
                ICommand cmd = this.CreateCommand();

                return cmd.ExecuteNonQuery(t);
            }

            return -1;
        }

        /// <summary>
        /// Update object
        /// </summary>
        public int Update(IEntity t)
        {
            if (t.IsPersisted)
            {
                ICommand cmd = this.CreateCommand();

                return cmd.ExecuteNonQuery(t);
            }

            return -1;
        }
        
        /// <summary>
        /// Delete data
        /// </summary>
        public int Delete<T>(T t) where T : IEntity
        {
            ICommand cmd = this.CreateCommand();

            return cmd.ExecuteDelete(t);
        }

        public virtual void Clear<T>() where T : IEntity
        { 
            IQuery query = this.CreateQuery<T>().Delete();

            this.ExecuteNonQuery(query.ToString(), null);
        } 

        #endregion

        #region Data Access   
        public abstract ICommand CreateCommand();

        public abstract DataSet ExecuteDataSet(string sql, params IDbDataParameter[] cmdParms);

        public virtual IDataReader ExecuteReader(IQuery query)
        {
            return this.ExecuteReader(query.ToString(), query.Parameters.ToArray());
        }

        public IDataReader ExecuteReader(string sql, params IDbDataParameter[] cmdParms)
        {
            //create a command 
            IDbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, this.Connection, this.OdbTransaction, sql, cmdParms);
         
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

        public T ExecuteScalar<T>(string sql, params IDbDataParameter[] cmdParms)
        {
            //create a command 
            IDbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, this.Connection, this.OdbTransaction, sql, cmdParms);

            //execute the command & return the results
            try
            {
                object retval = cmd.ExecuteScalar();

                cmd.Parameters.Clear();

                return (T)retval;
            }
            catch(Exception ex)
            {
                this.Connection.Close();

                throw ex;
            }           
        }

        public int ExecuteNonQuery(string sql, params IDbDataParameter[] cmdParms)
        {
            //create a command
            IDbCommand cmd = this.Connection.CreateCommand();
          
            SetCommand(cmd, this.Connection, this.OdbTransaction, sql, cmdParms);

            int n = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            //cmd.Dispose();

            return n;
        }

        protected static void SetCommand(IDbCommand cmd, IDbConnection conn, IDbTransaction trans, string cmdText, IDbDataParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = CommandType.Text;

            if (cmdParms != null)
            {
                foreach (IDbDataParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }

            return;
        }

        public int Delete(IEntity t)
        {
            throw new NotImplementedException();
        } 

        #endregion
    }
}
