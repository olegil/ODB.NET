using System.Data.SQLite;
using System.Data.ODB;

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

        public override string AddColumn(string name, string dbtype, ColumnAttribute colAttr)
        {
            string def = name + " " + dbtype;

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

            return def;
        }

        public virtual long Count<T>() where T : IEntity
        {
            IQuery q = this.Query<T>().Count("*").From();

            return this.ExecuteScalar<long>(q);
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
