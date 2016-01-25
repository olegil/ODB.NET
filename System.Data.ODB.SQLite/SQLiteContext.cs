﻿using System;
using System.Data.SQLite;

namespace System.Data.ODB.SQLite
{
    public class SQLiteContext : DbContext
    {
        public SQLiteContext(string conn)
            : base(new SQLiteConnection(conn))
        {        
        }

        public override IQuery<T> Query<T>()
        {
            return new SQLiteQuery<T>(this);
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

        public override long InsertReturnId<T>(T t)
        {
            if (this.Insert(t) > 0)
                return (this.Connection as SQLiteConnection).LastInsertRowId;

            return -1;
        }

        public override DataSet ExecuteDataSet(string sql, params IDbDataParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            IDbCommand cmd = this.Connection.CreateCommand();

            this.SetCommand(cmd, sql, commandParameters);

            //create the DataAdapter & DataSet 
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd as SQLiteCommand);

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
