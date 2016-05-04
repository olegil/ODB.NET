using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data.ODB;

namespace System.Data.ODB.SQLite
{
    public class SQLiteContext : OdbContext
    {
        public SQLiteContext(string conn) : base(new SQLiteConnection(conn))
        {        
        }
 
        public override IQuery<T> Query<T>()
        {      
            return new SQLiteQuery<T>(this); 
        }

        public override DataSet ExecuteDataSet(string sql, params IDbDataParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            IDbCommand cmd = this.Connection.CreateCommand();

            SetCommand(cmd, this.Connection, this.OdbTransaction, sql, commandParameters);

            //create the DataAdapter & DataSet 
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd as Data.SQLite.SQLiteCommand);

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

        public override ICommand CreateCommand()
        {
            return new OdbLiteCommand(this); 
        } 
    }
}
