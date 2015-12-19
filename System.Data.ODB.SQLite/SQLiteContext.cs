using System.Data.SQLite;
using System.Data.ODB;

namespace System.Data.ODB.SQLite
{
    public class SQLiteContext : DbContext
    {
        public SQLiteContext(string conn)
            : base(conn)
        {        
        }

        public override IDbConnection Init()
        {
            return new SQLiteConnection(this.ConnectionString);
        }

        public override IQuery<T> Query<T>()
        {
            return new SQLiteQuery<T>(this);
        }
         
        public override IDbDataParameter CreateParameter(ColumnMapping col)
        {
            SQLiteParameter p = new SQLiteParameter();

            p.ParameterName = "@" + col.Name;
            //p.Size = col.Attribute.Size;
            p.DbType = MappingHelper.TypeConvert(col.Value);  
            p.Value = col.Value;
             
            return p;
        }

        public override DataSet ExecuteDataSet(string sql, params IDbDataParameter[] commandParameters)
        {
            //create a command and prepare it for execution
            IDbCommand cmd = this.Connection.CreateCommand();

            this.SetCommand(cmd, sql, commandParameters);

            //create the DataAdapter & DataSet 
            IDataAdapter da = new SQLiteDataAdapter(cmd as SQLiteCommand);

            DataSet ds = new DataSet();

            try
            {
                //fill the DataSet using default values for DataTable names, etc.
                da.Fill(ds);

                cmd.Parameters.Clear();
            }
            catch
            {
                this.Connection.Close();

                throw;
            }

            //return the dataset
            return ds;
        }
    }
}
