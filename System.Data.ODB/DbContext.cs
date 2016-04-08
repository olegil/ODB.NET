﻿using System.Collections.Generic;
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
       
        public IQuery<T> Query<T>() where T : IEntity
        {
            TableVisitor tr = new TableVisitor(this.Depth);

            Type type = typeof(T);

            tr.Visit(type);

            IQuery<T> q = this.CreateQuery<T>().Select(tr.Colums);

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
        /// Create Table 
        /// </summary>
        /// 
        public int Create<T>() where T : IEntity
        {
            ICommand cmd = this.CreateCommand();

            return cmd.ExecuteCreate<T>();
        }
         
        /// <summary>
        /// Drop Table 
        /// </summary>
        public virtual int Remove<T>() where T : IEntity
        {
            ICommand cmd = this.CreateCommand();

            return cmd.ExecuteDrop<T>();
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
        /// Store object
        /// </summary>
        public int Insert(IEntity t)
        {
            ICommand cmd = this.CreateCommand();

            return cmd.ExecuteInsert(t); 
        }

        public int Update(IEntity t)
        {
            ICommand cmd = this.CreateCommand();

            return cmd.ExecuteUpdate(t);
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
