using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public abstract class OdbContext : IContext
    {
        public int Depth { get; set; }

        public IDbConnection Connection { get; set; }
        public IDbTransaction Transaction { get; set; }

        private bool disposed = false;
     
        public OdbContext(IDbConnection connection)
        {
            this.Connection = connection;

            this.Depth = 1;
        }
 
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
 
        public void Close()
        {
            if (this.Transaction != null)
            {
                this.Transaction.Dispose();

                this.Transaction = null;
            }

            if (this.Connection != null)
            {
                if (this.Connection.State == ConnectionState.Open)
                    this.Connection.Close();                               
            }
        }
        
        #region Transaction

        public virtual void StartTrans()
        {
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();

            this.Transaction = this.Connection.BeginTransaction(); 
        }

        public virtual void CommitTrans()
        {
            try
            {
                this.Transaction.Commit();       
            }
            catch
            {
                throw new OdbException("Transaction commit fail.");
            }
        }

        public virtual void RollBack()
        {
            try
            {
                this.Transaction.Rollback();
            }
            catch
            {
                throw new OdbException("Transaction commit fail."); 
            }
        }

        #endregion
              
        public abstract IQuery Query();

        public virtual IQuery Select<T>() where T : IEntity
        {
            Type type = typeof(T);
 
            OdbDiagram dg = new OdbDiagram(this.Depth);
            dg.CreateTableList(type);
                    
            IQuery q = this.Query();

            q.Diagram = dg;

            OdbTable table = dg.GetTable(type);
 
            q.Select(dg.GetColumns(type)).From(table.Name, table.Alias);
            q.Append(dg.GetChildNodes(table));
 
            return q;
        }

        /// <summary>
        /// Create Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public virtual void ExecuteCreate<T>() where T : IEntity
        {
            this.Create(typeof(T));
        }
        
        public abstract void Create(string table, string[] cols);

        private void Create(Type type)
        { 
            List<string> cols = new List<string>();

            foreach (OdbColumn col in OdbMapping.GetColumns(type))
            { 
                if (col.Attribute.IsModel)
                {   
                    this.Create(col.GetMapType());
                }
 
                cols.Add(this.SqlDefine(col));                 
            }

            string table = OdbMapping.GetTableName(type);

            this.Create(table, cols.ToArray());
        } 
    
        /// <summary>
        /// Drop Table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public virtual void ExecuteDrop<T>() where T : IEntity
        { 
            this.Drop(typeof(T));
        }

        public abstract void Drop(string table);

        private void Drop(Type type)
        {
            foreach (OdbColumn col in OdbMapping.GetColumns(type))
            {
                if (col.Attribute.IsModel)
                {
                    this.Drop(col.GetMapType());
                } 
            } 

            string table = OdbMapping.GetTableName(type);

            this.Drop(table);
        }

        /// <summary>
        /// Get result list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="q"></param>
        /// <returns></returns>
        public virtual IList<T> ExecuteList<T>(IQuery query) where T : IEntity
        {
            using (IDataReader rdr = this.ExecuteReader(query.ToString(), query.Parameters.ToArray()))
            {
                IList<T> list = new List<T>();

                OdbEntityReader<T> odr = new OdbEntityReader<T>(rdr, query.Diagram);
 
                try
                {
                    foreach (T t in odr)
                    {
                        list.Add(t);
                    }
                }
                catch
                {
                    throw;
                }
                
                return list;
            }
        }

        public void ExecutePersist<T>(T t) where T : IEntity
        { 
            OdbWriter wr = new OdbWriter(this);

            wr.Write(t);                
        }
 
        /// <summary>
        /// Delete object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public int ExecuteDelete<T>(T t) where T : IEntity
        {
            return this.Query().Delete<T>().Where("Id").Eq(t.Id).Execute();
        }
        
        public abstract IDbDataParameter CreateParameter();

        public abstract string TypeMapping(DbType dtype);

        public abstract string SqlDefine(OdbColumn col);

        #region Data access

        protected static IDbCommand SetCommand(IDbConnection conn, IDbTransaction trans, string cmdText, IDbDataParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();

            IDbCommand cmd = conn.CreateCommand();

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

            return cmd;
        }
     
        public IDataReader ExecuteReader(string sql, params IDbDataParameter[] cmdParms)
        {
            //create a command 
            IDbCommand cmd = SetCommand(this.Connection, this.Transaction, sql, cmdParms);

            try
            {
                IDataReader rdr = cmd.ExecuteReader();

                cmd.Parameters.Clear();

                return rdr;
            }
            catch
            { 
                throw;
            }
        } 

        public T ExecuteScalar<T>(string sql, params IDbDataParameter[] cmdParms)
        {
            //create a command 
            IDbCommand cmd = SetCommand(this.Connection, this.Transaction, sql, cmdParms);

            //execute the command & return the results
            try
            {
                object retval = cmd.ExecuteScalar();

                cmd.Parameters.Clear();

                return (T)retval;
            }
            catch (Exception ex)
            {
                this.Connection.Close();

                throw ex;
            }
        }

        public int ExecuteNonQuery(string sql, params IDbDataParameter[] cmdParms)
        {
            //create a command
            IDbCommand cmd = SetCommand(this.Connection, this.Transaction, sql, cmdParms);

            int n = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            cmd.Dispose();

            return n;
        }
         
        #endregion
    }
}
