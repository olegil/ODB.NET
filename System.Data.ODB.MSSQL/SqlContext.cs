using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace System.Data.ODB.MSSQL
{
    public class SqlContext : DbContext
    {
        public SqlContext(string conn)
            : base(new SqlConnection(conn))
        {        
        }

        public override IQuery<T> CreateQuery<T>()
        {
            return this.CreateQuery<T>("");
        }

        public override IQuery<T> CreateQuery<T>(string sql)
        {
            SqlQuery<T> q = new SqlQuery<T>();

            q.Db = this;
            q.Append(sql);

            return q;
        }

        public override ICommand CreateCommand()
        {
            return new OdbSqlCommand(this);
        }

        public override DataSet ExecuteDataSet(string sql, params IDbDataParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            IDbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, this.Connection, this.OdbTransaction, sql, commandParameters);

            //create the DataAdapter & DataSet 
            SqlDataAdapter da = new SqlDataAdapter(cmd as SqlClient.SqlCommand);

            DataSet ds = new DataSet();

            try
            {
                //fill the DataSet using default values for DataTable names
                da.Fill(ds);

                cmd.Parameters.Clear();
            }
            catch
            {
                this.Connection.Close();

                throw;
            }
            finally
            {
                da.Dispose();
            }

            //return the dataset
            return ds;
        } 
    }
}
