using System;
using System.Collections.Generic; 

namespace System.Data.ODB
{
    public interface IDbContext 
    {
        int Depth { get; set; }

        int Create<T>(bool isCascade) where T : IEntity;

        int Remove<T>(bool isCascade) where T : IEntity;

        IList<T> Get<T>(IQuery query) where T : IEntity;
  
        DataSet ExecuteDataSet(IQuery query);
 
        IDataReader ExecuteReader(string sql, params IDbDataParameter[] commandParameters);

        int ExecuteNonQuery(IQuery query);

        int ExecuteNonQuery(string sql, params IDbDataParameter[] commandParameters);

        T ExecuteScalar<T>(IQuery query); 
    }
}
