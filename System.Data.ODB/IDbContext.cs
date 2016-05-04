using System;
using System.Collections.Generic; 

namespace System.Data.ODB
{
    public interface IDbContext 
    {
        int Depth { get; set; }

        void Create<T>() where T : IEntity;

        void Remove<T>() where T : IEntity;

        int Insert<T>(T t) where T : IEntity;

        int Update<T>(T t) where T : IEntity;

        int Delete<T>(T t) where T : IEntity;

        IQuery<T> Query<T>() where T : IEntity;
                
        IList<T> Get<T>(IQuery query) where T : IEntity;

        IDbConnection Connection { get; set; }

        ICommand CreateCommand();

        DataSet ExecuteDataSet(string sql, params IDbDataParameter[] commandParameters);
 
        IDataReader ExecuteReader(string sql, params IDbDataParameter[] commandParameters);

        int ExecuteNonQuery(string sql, params IDbDataParameter[] commandParameters);

        T ExecuteScalar<T>(string sql, params IDbDataParameter[] commandParameters); 
    }
}
