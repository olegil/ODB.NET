using System; 
using System.Collections.Generic;

namespace System.Data.ODB
{
    public interface IQuery<T> where T : IEntity
    {
        IQuery<T> Create();
        IQuery<T> Drop();
        IQuery<T> Select(string[] cols);
        IQuery<T> From();
        IQuery<T> Insert(string[] cols);
        IQuery<T> Update();
        IQuery<T> Delete();
        IQuery<T> Values(string[] cols);
        IQuery<T> Set(string[] cols);
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
        IQuery<T> Skip(int start);
        IQuery<T> Take(int count);
        IQuery<T> Join<T1>() where T1 : IEntity;
        IQuery<T> LeftJoin<T1>() where T1 : IEntity;
        IQuery<T> As(string str);
        IQuery<T> On(string str);
        IQuery<T> Equal(string str);
        IQuery<T> SortAsc();
        IQuery<T> SortDesc();

        IQuery<T> Append(string str);

        IDbDataParameter BindParam(string name, object b, ColumnAttribute attr);

        List<IDbDataParameter> Parameters { get; set; }

        T First();

        List<T> ToList();

        string ToString();
    }
}
