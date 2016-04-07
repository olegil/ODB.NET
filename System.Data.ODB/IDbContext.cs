﻿using System;
using System.Collections.Generic; 

namespace System.Data.ODB
{
    public interface IDbContext 
    {
        int Depth { get; set; }

        int Create<T>() where T : IEntity;

        int Remove<T>() where T : IEntity;

        int Delete<T>(T t) where T : IEntity;

        int Insert(IEntity t);

        int Update(IEntity t);

        IQuery<T> Query<T>() where T : IEntity;

        IList<T> Get<T>(IQuery query) where T : IEntity;

        IDbConnection Connection { get; set; }
        IOdbCommand CreateCommand();

        DataSet ExecuteDataSet(string sql, params IDbDataParameter[] commandParameters);
 
        IDataReader ExecuteReader(string sql, params IDbDataParameter[] commandParameters);

        int ExecuteNonQuery(string sql, params IDbDataParameter[] commandParameters);

        T ExecuteScalar<T>(string sql, params IDbDataParameter[] commandParameters); 
    }
}
