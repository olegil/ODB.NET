using System; 
using System.Collections.Generic;

namespace System.Data.ODB
{ 
    public interface IQuery 
    {      
        IQuery Insert(string table, string[] cols);
        IQuery Values(string[] cols);             
        IQuery Update(string table);
        IQuery Delete<T>() where T : IEntity;        
        IQuery Select(string[] cols);
        IQuery From<T>() where T : IEntity;
        IQuery From(string table, string alias = "");
        IQuery Where(string str);
        IQuery And(string str);
        IQuery Or(string str);
        IQuery OrderBy(string str);
        IQuery Eq(object val);
        IQuery NotEq(object val);
        IQuery Gt(object val);
        IQuery Lt(object val);
        IQuery Gte(object val);
        IQuery Lte(object val);
        IQuery Not();
        IQuery Like(string str);
        IQuery Set(string[] cols);
        IQuery Count(string str);
        IQuery Skip(int start);
        IQuery Take(int count);
        IQuery Join<T>() where T : IEntity;
        IQuery Join(string table);
        IQuery LeftJoin<T>() where T : IEntity;
        IQuery LeftJoin(string table);
        IQuery As(string str);
        IQuery On(string str);
        IQuery Equal(string str);
        IQuery SortAsc();
        IQuery SortDesc();

        IQuery Append(string str);
        List<IDbDataParameter> Parameters { get; set; }

        OdbDiagram Diagram { get; set; }

        DataSet Result();
        T First<T>() where T : IEntity;
        List<T> ToList<T>() where T : IEntity;
        int Execute();
        int ExecuteReturnId();
        int Single();
     
        string ToString();
    }
}
