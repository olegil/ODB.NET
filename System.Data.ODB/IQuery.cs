using System; 
using System.Collections.Generic;

namespace System.Data.ODB
{
    public interface IQuery<T> where T : IEntity
    {
        IQuery<T> Select(string[] cols);
        IQuery<T> From();
        IQuery<T> Insert(string table);
        IQuery<T> Update(string table);
        IQuery<T> Delete();
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
        IQuery<T> Like(string str);
        IQuery<T> Count();
        IQuery<T> Join<T2>() where T2 : IEntity;
        IQuery<T> LeftJoin<T2>() where T2 : IEntity;
        IQuery<T> As(string str);
        IQuery<T> On(string str);
        IQuery<T> Equal(string str);
        IQuery<T> SortAsc();
        IQuery<T> SortDesc();
        IQuery<T> Set(string str);
        IQuery<T> Symbol(string str);
        IQuery<T> Values(string str);

        void AddParameter(IDbDataParameter p);

        IDbDataParameter[] GetParameters();

        List<T> ToList();

        T First();

        DataTable GetTable();

        int ExecuteCommand();

        string ToString();
    }
}
