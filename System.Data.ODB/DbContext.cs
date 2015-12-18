using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace System.Data.ODB
{
    public abstract class DbContext : IDbContext, IDisposable       
    {
        public string ConnectionString { get; private set; }
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

        protected IDbConnection _conn;
        public IDbConnection Connection
        {
            get
            {
                if (this._conn == null)
                    this._conn = this.Init();

                return this._conn;
            }
        }

        private IDbTransaction _trans { get; set; }

        private bool _inTrans;
        public bool InTransaction { get { return this._inTrans; } }

        public DbContext(string dbconn)
        {
            this.ConnectionString = dbconn;

            this.IsEntityTracking = false;

            this.DbState = new Dictionary<string, EntityState>();
        }

        public abstract IDbConnection Init();

        public void Close()
        {
            if (this._trans != null)
            {
                this._trans.Dispose();

                this._trans = null;
            }

            if (this._conn != null)
            {
                if (this._conn.State == ConnectionState.Open)
                    this._conn.Close();
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

        public virtual bool Create<T>() where T : IEntity
        {
            Type type = typeof(T);

            string table = MappingHelper.GetTableName(type);

            List<string> fields = new List<string>();

            StringBuilder sb = new StringBuilder(); 

            foreach (PropertyInfo pi in type.GetProperties())
            {
                ColumnAttribute colAttr = MappingHelper.GetColumnAttribute(pi);

                if (colAttr != null)
                {
                    string def = colAttr.Name == "" ? pi.Name : colAttr.Name;

                    def += " " + MappingHelper.DataConvert(pi.PropertyType);               

                    if (colAttr.IsPrimaryKey)
                    {
                        def += " PRIMARY KEY";
                    }

                    if (colAttr.IsAuto)
                    {
                        def += " AUTOINCREMENT";
                    }

                    if (colAttr.IsNullable)
                    {
                        def += " NULL";
                    }
                    else
                    {
                        def += " NOT NULL";
                    }

                    fields.Add(def);                     
                }
            }

            if (fields.Count > 0)
            {
                sb.AppendLine("CREATE TABLE IF NOT EXISTS \"" + table + "\"(");
                sb.Append(string.Join(",\r\n", fields.ToArray()));
                sb.AppendLine("\r\n);");
            }

            this.ExecuteNonQuery(sb.ToString(), null);

            return true;
        }

        public virtual bool Drop<T>() where T : IEntity
        {
            Type type = typeof(T);
 
            string def = "DROP TABLE IF EXISTS " + MappingHelper.GetTableName(type) + ";";

            this.ExecuteNonQuery(def, null);

            return true;
        }

        public virtual IQuery<T> Table<T>() where T : IEntity
        { 
            return this.Select<T>(new[] { "*" });
        }

        public virtual IQuery<T> Select<T>(string[] cols) where T : IEntity
        { 
            return this.BuildQuery<T>().Select(cols).From();
        } 

        public virtual IList<T> Get<T>(IQuery<T> q) where T : IEntity
        {
            using (IDataReader rdr = this.ExecuteReader(q))
            {
                return this.Get<T>(rdr);               
            }
        }

        public IList<T> Get<T>(IDataReader rdr) where T : IEntity
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

                        if (attr.EnumMapping)
                        {
                            pi.SetValue(instance, Enum.Parse(pi.PropertyType, value.ToString(), false), null);
                        }
                        else
                        {
                            pi.SetValue(instance, value, null);
                        }
                    }

                    if (pi.Name == "IsPersisted")
                    {
                        pi.SetValue(instance, true, null);
                    }
                }

                //FieldInfo fi = type.BaseType.GetField("_oncreate", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                //fi.SetValue(t, false);
            
                list.Add(instance); 

                if (IsEntityTracking)
                {
                    this.DbState.Add(instance.EntityId, new EntityState(instance));
                }
            }

            rdr.Close();
            
            return list;
        }
                       
        public virtual int Insert<T>(T t) where T : IEntity
        {
            if (t == null)
            {
                throw new Exception("No object to insert.");
            }
             
            List<string> fields = new List<string>();
            List<string> ps = new List<string>();

            TableMapping table = MappingHelper.Create<T>(t);

            IQuery<T> sql = this.BuildQuery<T>().Insert(table.Name);

            foreach (ColumnMapping col in table.Columns)
            {
                if (!col.Attribute.IsAuto)
                {
                    IDbDataParameter pr = this.CreateParameter(col);

                    sql.AddParameter(pr);

                    fields.Add(col.Name);
                    ps.Add(pr.ParameterName);
                }                
            }

            sql.Symbol(" (" + string.Join(", ", fields.ToArray()) + ")");         
            sql.Values(string.Join(", ", ps.ToArray()));

            return this.ExecuteNonQuery(sql.ToString(), sql.GetParameters()); 
        }
 
        public virtual int Update<T>(T t) where T : IEntity
        {
            if (t == null || !t.IsPersisted)
            {
                throw new Exception("No object to update.");
            }

            TableMapping table = MappingHelper.Create<T>(t);

            ColumnMapping colKey = table.PrimaryKey;

            if (colKey == null)
                throw new Exception("No key column.");

            List<string> fields = new List<string>();

            IQuery<T> query = this.BuildQuery<T>().Update(table.Name);

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

            query.Set(string.Join(", ", fields.ToArray())).Where(colKey.Name).Eq(colKey.Value);

            if (fields.Count > 0)
                return this.ExecuteNonQuery(query.ToString(), query.GetParameters());
            else
                return this.ExecuteNonQuery("");
        }

        public virtual int Delete<T>(T t) where T : IEntity
        {
            if (t == null || !t.IsPersisted)
                return 0;

            TableMapping table = MappingHelper.Create<T>(t);

            ColumnMapping colKey = table.PrimaryKey;

            if (colKey == null)
            {
                throw new Exception("No key column.");
            }

            IQuery<T> query = this.BuildQuery<T>().Delete().Where(colKey.Name).Eq(colKey.Value);

            return this.ExecuteNonQuery(query.ToString(), query.GetParameters());          
        }

        public int Clear<T>() where T : IEntity
        {
            IQuery<T> q = this.BuildQuery<T>().Delete();

            return this.ExecuteNonQuery(q);
        }

        public IQuery<T> Count<T>() where T : IEntity
        {
            return this.BuildQuery<T>().Count();
        }

        public abstract IQuery<T> BuildQuery<T>() where T : IEntity;
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
            //create a command and prepare it for execution
            IDbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, sql, commandParameters);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
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
            //create a command and prepare it for execution
            IDbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, sql, commandParameters);

            //execute the command & return the results
            T retval = (T)cmd.ExecuteScalar();

            // detach the SqlParameters from the command object, so they can be used again.
            cmd.Parameters.Clear();

            return retval;
        }

        public virtual int ExecuteNonQuery<T>(IQuery<T> query) where T : IEntity
        {
            return this.ExecuteNonQuery(query.ToString(), query.GetParameters());
        }

        public int ExecuteNonQuery(string sql, params IDbDataParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            IDbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, sql, commandParameters);

            int n = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            return n;
        }

        protected void SetCommand(IDbCommand cmd, string cmdText, IDbDataParameter[] commandParameters)
        {
            //Open the connection if required
            if (this.Connection.State != ConnectionState.Open)
                this.Connection.Open();

            //Set up the command
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
