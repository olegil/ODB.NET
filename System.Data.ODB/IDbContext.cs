using System;
using System.Collections.Generic; 

namespace System.Data.ODB
{
    public interface IDbContext 
    {
        int Depth { get; set; }

        void Create<T>() where T : IEntity;

        void Remove<T>() where T : IEntity;

        int Insert(IEntity t);

        int Update(IEntity t);

        int Delete(IEntity t);

        IQuery<T> CreateQuery<T>() where T : IEntity;

        IQuery<T> CreateQuery<T>(string sql) where T : IEntity;

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
