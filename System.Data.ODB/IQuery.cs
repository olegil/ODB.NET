using System; 
using System.Collections.Generic;

namespace System.Data.ODB
{
    public interface IQuery
    { 
        IQuery Insert(string[] cols);
        IQuery Values(string[] cols);              
        IQuery Append(string str);

        string Define(string name, string dbtype, ColumnAttribute colAttr);
        void AddParam(string name, object b, ColumnAttribute attr);

        IDbDataParameter[] GetParams();
 
        DataSet Result();

        string ToString();
    }

    public interface IQuery<T> : IQuery
    { 
        string Table { get; set; }

        int Create();
        int Drop();

        IQuery<T> Delete();
        IQuery<T> Update();
        IQuery<T> Select(string[] cols);
        IQuery<T> From();
        IQuery<T> From(string table);
        IQuery<T> Where(string str);
        IQuery<T> And(string str);
        IQuery<T> Or(string str);
        IQuery<T> OrderBy(string str);
        IQuery<T> Eq(object val);
        IQuery<T> NotEq(object val);
        IQuery<T> Gt(object val);
        IQuery<T> Lt(object val);
        IQuery<T> Gte(object val);
        IQuery<T> Lte(object val);
        IQuery<T> Not(string str);
        IQuery<T> Like(string str);
        IQuery<T> Set(string[] cols);
        IQuery<T> Count(string str);
        IQuery<T> Skip(int start);
        IQuery<T> Take(int count);
        IQuery<T> Join<T1>() where T1 : IEntity;
        IQuery<T> Join(string table);
        IQuery<T> LeftJoin<T1>() where T1 : IEntity;
        IQuery<T> LeftJoin(string table);
        IQuery<T> As(string str);
        IQuery<T> On(string str);
        IQuery<T> Equal(string str);
        IQuery<T> SortAsc();
        IQuery<T> SortDesc();

        T First();

        List<T> ToList();

        long ToInt();               
    }
}
