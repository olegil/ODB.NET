using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public abstract class OdbContext : IDbContext, IDisposable
    {
        protected IDbConnection Connection { get; set; }
        protected IDbTransaction Transaction { get; set; }
 
        public int Depth { get; set; }

        private bool _inTrans;

        private bool disposed = false;
        
        public OdbContext(IDbConnection Connection, int depth)
        {
            this.Connection = Connection;
            this.Depth = depth;
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

                this.Connection = null;
            }
        }

        #region Transaction

        public void StartTrans()
        {
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();

            this.Transaction = this.Connection.BeginTransaction();

            this._inTrans = true;
        }

        public void CommitTrans()
        {
            try
            {
                this.Transaction.Commit();

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
                this.Transaction.Rollback();

                this._inTrans = false;
            }
            catch
            {
                throw;
            }
        }

        #endregion
              
        public abstract IQuery Query();

        public virtual void ExecuteCreate<T>() where T : IEntity
        {
            this.Create(typeof(T));
        }
        
        public abstract void Create(string table, string[] cols);

        private void Create(Type type)
        { 
            List<string> cols = new List<string>();

            foreach (OdbColumn col in OdbMapping.GetColumn(type))
            { 
                if (col.Attribute.IsForeignkey)
                {   
                    this.Create(col.GetColumnType());
                }
 
                cols.Add(this.SqlDefine(col));                 
            }

            string table = OdbMapping.GetTableName(type);

            this.Create(table, cols.ToArray());
        } 
    
        public virtual void ExecuteDrop<T>() where T : IEntity
        { 
            this.Drop(typeof(T));
        }

        public abstract void Drop(string table);

        private void Drop(Type type)
        {
            foreach (OdbColumn col in OdbMapping.GetColumn(type))
            {
                if (col.Attribute.IsForeignkey)
                {
                    this.Drop(col.GetColumnType());
                } 
            } 

            string table = OdbMapping.GetTableName(type);

            this.Drop(table);
        }

        /// <summary>
        /// Get query result
        /// </summary>    
        public virtual IList<T> ExecuteList<T>(IQuery q) where T : IEntity
        {
            using (IDataReader rdr = this.ExecuteReader(q.ToString(), q.Parameters.ToArray()))
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
 
        /// <summary>
        /// Persistence object
        /// </summary>
        public virtual int ExecutePersist<T>(T t) where T : IEntity
        {  
            Type type = t.GetType();

            IQuery query = this.Query();

            int n = 0;

            List<string> cols = new List<string>();
            List<string> ps = new List<string>();

            OdbColumn ColPk = null;
             
            //begin foreach
            foreach (OdbColumn col in OdbMapping.GetColumn(type))
            {
                ColumnAttribute attr = col.Attribute;

                if (!attr.IsAuto)
                {
                    object b = col.GetValue(t);

                    if (!attr.IsForeignkey)
                    {
                        if (b == null)                           
                            b = DBNull.Value;
                    }
                    else
                    { 
                        if (this.Depth > 1 )
                        {
                            if (b != null)
                            {
                                IEntity entry = b as IEntity;

                                this.Depth--;

                                //return id
                                b = this.ExecutePersist(entry);

                                this.Depth++;
                            }                     
                        }
                    }

                    string name = "@p" + n;

                    IDbDataParameter p = this.CreateParameter();

                    p.ParameterName = name;
                    p.Value = b;
                    p.DbType = col.GetDbType();

                    query.Parameters.Add(p);

                    ps.Add(name);
                    cols.Add("[" + col.Name + "]");

                    n++;
                }

                if (attr.IsPrimaryKey)
                {
                    ColPk = col;
                }
            }
            //end

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
            
                query.Update<T>().Set(cols.ToArray()).Where(ColPk.Name).Eq(ColPk.GetValue(t));

                this.ExecuteNonQuery(query.ToString(), query.Parameters.ToArray());

                return (int)t.Id; 
            }
            else
            {
                query.Insert<T>(cols.ToArray()).Values(ps.ToArray());

                return this.ExecuteInsertId(query);
            } 
        }

        public abstract int ExecuteInsertId(IQuery q);
        
        public abstract IDbDataParameter CreateParameter();

        public abstract string TypeMapping(DbType dtype);

        public abstract string SqlDefine(OdbColumn col);

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
         
        public abstract DataSet ExecuteDataSet(string sql, params IDbDataParameter[] cmdParms);
 
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
                this.Connection.Close();

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
    }
}
