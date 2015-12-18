using System;
using System.Collections.Generic; 

namespace System.Data.ODB
{
    public interface IDbContext 
    {
        bool Create<T>() where T : IEntity;

        IQuery<T> BuildQuery<T>() where T : IEntity;
       
        IList<T> Get<T>(IQuery<T> query) where T : IEntity;

        DataSet ExecuteDataSet<T>(IQuery<T> query) where T : IEntity;

        IDataReader ExecuteReader<T>(IQuery<T> query) where T : IEntity;

        int ExecuteNonQuery<T>(IQuery<T> query) where T : IEntity;        
    }
}
