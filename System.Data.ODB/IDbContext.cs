using System;
using System.Collections.Generic; 

namespace System.Data.ODB
{
    public interface IDbContext 
    {
        int Create<T>() where T : IEntity;

        int Remove<T>() where T : IEntity;

        IList<T> Get<T>(IQuery query) where T : IEntity;
  
        DataSet ExecuteDataSet(IQuery query);

        IDataReader ExecuteReader(IQuery query);

        int ExecuteNonQuery(IQuery query);

        T ExecuteScalar<T>(IQuery query);
    }
}
