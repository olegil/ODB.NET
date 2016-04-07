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

        public IDbConnection Connection { get; set; }

        public IDbTransaction OdbTransaction { get; set; }

        private bool _inTrans;
        public bool InTransaction { get { return this._inTrans; } }

        public DbContext(IDbConnection DbConnection)
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

        public abstract IQuery<T> Query<T>() where T : IEntity;

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

        /// <summary>
        /// Create Table 
        /// </summary>
        /// 
        public abstract int Create<T>() where T : IEntity;

        public virtual int Create(string table, string[] cols)
        {
            string sql = "CREATE TABLE IF NOT EXISTS \"" + table + "\" (\r\n" + string.Join(",\r\n", cols) + "\r\n);";

            return this.ExecuteNonQuery(sql);
        }

        /// <summary>
        /// Drop Table 
        /// </summary>
        public abstract int Remove<T>() where T : IEntity;
         
        public virtual int Drop(string table)
        {
            return this.ExecuteNonQuery("DROP TABLE IF EXISTS \"" + table + "\"");
        }
              
        public virtual IQuery<T> Count<T>(string str) where T : IEntity
        {
            IQuery<T> q = this.Query<T>().Count(str).From();

            return q;
        }

        public virtual IQuery<T> Count<T>() where T : IEntity
        {
            return this.Count<T>("*");
        } 

        /// <summary>
        /// Select from Table
        /// </summary>
        public virtual IQuery<T> Get<T>() where T : IEntity
        { 
            TableVisitor tr = new TableVisitor(this.Depth);

            Type type = typeof(T);

            tr.Visit(type);

            IQuery<T> q = this.Query<T>().Select(tr.Colums);

            q.Alias = "T0";

            foreach (KeyValuePair<string, string> tc in tr.Tables)
            {
                if (tc.Value == "")
                    q.From(tc.Key);
                else
                    q.LeftJoin(tc.Key).On(tc.Value);
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
                IList<T> list = new List<T>();

                using (EntityReader<T> edr = new EntityReader<T>(rdr, this.Depth))
                {
                    foreach (T t in edr)
                    {
                        list.Add(t);
                    } 
                }

                return list;             
            }
        }  

        /// <summary>
        /// Store object
        /// </summary>
        public int Insert(IEntity t)
        {
            IOdbCommand cmd = this.CreateCommand();

            return cmd.Insert(t); 
        }

        public int Update(IEntity t)
        {
            IOdbCommand cmd = this.CreateCommand();

            return cmd.Update(t);
        }
    
        /// <summary>
        /// Delete all table data
        /// </summary>
        public virtual bool Clear<T>() where T : IEntity
        {
            IQuery q = this.Query<T>().Delete();

            this.ExecuteNonQuery(q);

            return true;
        }

        public int Delete<T>(T t) where T : IEntity
        {
            IOdbCommand cmd = this.CreateCommand();

            return cmd.Delete(t);
        }

        #endregion

        #region Data Access   
        public abstract IOdbCommand CreateCommand();

        public abstract DataSet ExecuteDataSet(string sql, params IDbDataParameter[] cmdParms);

        public virtual IDataReader ExecuteReader(IQuery query)
        {
            return this.ExecuteReader(query.ToString(), query.GetParams());
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
            return this.ExecuteScalar<T>(query.ToString(), query.GetParams());
        }

        public T ExecuteScalar<T>(string sql, params IDbDataParameter[] cmdParms)
        {
            //create a command 
            IDbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, this.Connection, this.OdbTransaction, sql, cmdParms);

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
            return this.ExecuteNonQuery(query.ToString(), query.GetParams());
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

        #endregion
    }
}
