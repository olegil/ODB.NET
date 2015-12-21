using System;
using System.Collections.Generic; 

namespace System.Data.ODB
{
    public interface IDbContext 
    { 
        int Create<T>() where T : IEntity;
         
        IList<T> Get<T>(IQuery query) where T : IEntity;

        DataSet ExecuteDataSet(IQuery query);

        IDataReader ExecuteReader<T>(IQuery query) where T : IEntity;

        int ExecuteNonQuery(IQuery query);       
    }
}
