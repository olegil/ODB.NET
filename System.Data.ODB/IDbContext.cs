using System;
using System.Collections.Generic; 

namespace System.Data.ODB
{
    public interface IDbContext 
    {
        int Create<T>() where T : IEntity;

        int Remove<T>() where T : IEntity;

        IList<T> Get<T>(IQuery query) where T : IEntity;
  
        DataSet ExecuteDataSet(string sql, params IDbDataParameter[] commandParameters);

        DataSet ExecuteDataSet(IQuery query);

        IDataReader ExecuteReader(string sql, params IDbDataParameter[] commandParameters);

        int ExecuteNonQuery(string sql, params IDbDataParameter[] commandParameters);

        T ExecuteScalar<T>(IQuery query);

        T ExecuteScalar<T>(string sql, params IDbDataParameter[] commandParameters);
    }
}
