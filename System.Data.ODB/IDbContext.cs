using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Data.ODB
{
    public interface IDbContext
    {
        int Depth { get; set; }

        IQuery Query();
     
        void ExecuteCreate<T>() where T : IEntity;
        void ExecuteDrop<T>() where T : IEntity;
        int ExecutePersist<T>(T t) where T : IEntity;
        IList<T> ExecuteList<T>(IQuery q) where T : IEntity;
   
        int ExecuteNonQuery(string sql, params IDbDataParameter[] cmdParms);
        IDataReader ExecuteReader(string sql, params IDbDataParameter[] cmdParms);
        DataSet ExecuteDataSet(string sql, params IDbDataParameter[] cmdParms);

        IDbDataParameter CreateParameter();
    }
}
